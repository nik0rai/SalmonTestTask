using ContactsSyncService.Data;
using ContactsSyncService.Services;
using ContactsSyncService.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace ContactsSyncService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Produces("application/json")]
    public class ContactsSyncController : ControllerBase
    {
        //private readonly ILogger<ContactsSyncController> _logger;   // Dependency Injection logger
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration _configuration;             // Dependency Injection configuration
        private readonly ApiDbContext _dbContext;                   // Dependency Injection database connector
        private readonly TokenBuilderService _tokenBuilderService;  // Dependency Injection token generator service
        public ContactsSyncController(IConfiguration configuration, ApiDbContext context, TokenBuilderService tokenBuilderService)
        {
            _logger.Info("Controller created!");
            _configuration = configuration;
            _dbContext = context;
            _tokenBuilderService = tokenBuilderService;
        }

        /// <summary>
        /// Проверить работает ли API.
        /// </summary>
        /// <returns>Api ok если все работает.</returns>
        [HttpGet("/")]
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public IActionResult CheckApi()
        {
            _logger.Info("Checking Api");
            try
            {
                return Ok("Api Ok");
            }
            catch
            {
                _logger.Error("Service is not available!");
                return Problem("Service is not available!", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
        }

        /// <summary>
        /// Получить историю покупок контакта.
        /// </summary>
        /// <param name="key">Email или телефон.</param>
        /// <param name="fullName">Полное имя.</param>
        /// <returns>Сообщение сколько записей есть, а также сами записи.</returns>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentHistory(string key, string fullName)
        {
            _logger.Info($"{nameof(GetPaymentHistory)} is called.");

            #region Validate input values

            var isValidInputVals = Validator.IsValid(Validator.Rule.StringNotEmpty, key, fullName);

            if (!isValidInputVals.IsActive)
            {
                _logger.Error($"Input values cannot be empty or null: {string.Join(", ", isValidInputVals.Errors)}!");
                return BadRequest(new { error = true, message = $"{string.Join(", ", isValidInputVals.Errors)} cannot be empty!" });
            }

            #endregion

            #region If user is not active

            var contactActivity = await CheckIsActive(key, fullName);

            if (contactActivity.error)
            {
                _logger.Error($"Contact {fullName} with {key} does not exist!");
                return NotFound(contactActivity);
            }

            if (!contactActivity.IsActive)
            {
                _logger.Error($"Contact {fullName} with {key} is not active!");
                return Ok(new { error = false, message = "Contact is not active.", newState = contactActivity });
            }

            #endregion

            var contact = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.Email.Equals(key) || x.PhoneNumber.Equals(key));

            var history = await _dbContext.Payments.Where(x => x.Email.Equals(contact.Email) || x.PhoneNumber.Equals(contact.PhoneNumber)).ToListAsync();

            return Ok(new { error = false, message = $"Contact has {history.Count} rows of history", addHistory = history });
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFound), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContactInfo(string key, string fullName)
        {
            _logger.Info($"{nameof(GetContactInfo)} is called.");

            #region Validate input values

            var isValidInputVals = Validator.IsValid(Validator.Rule.StringNotEmpty, key, fullName);

            if (!isValidInputVals.IsActive)
            {
                _logger.Error($"Input values cannot be empty or null: {string.Join(", ", isValidInputVals.Errors)}!");
                return Ok(new { error = true, message = $"{string.Join(", ", isValidInputVals.Errors)} cannot be empty!" });
            }

            #endregion

            #region If user is not active

            var contactActivity = await CheckIsActive(key, fullName);

            if (contactActivity.error)
            {
                _logger.Error($"Contact {fullName} with {key} does not exist!");
                return Ok(contactActivity);
            }

            if (!contactActivity.IsActive)
            {
                _logger.Error($"Contact {fullName} with {key} is not active!");
                return Ok(new { error = false, message = "Contact is not active.", newState = contactActivity });
            }

            #endregion

            var contact = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.Email.Equals(key) || x.PhoneNumber.Equals(key));

            var contactInfo = await _dbContext.Address.Where(x => x.Email.Equals(contact.Email) || x.PhoneNumber.Equals(contact.PhoneNumber)).ToListAsync();

            return Ok(new { error = false, message = $"Contact has {contactInfo.Count} rows of information", addContactInfo = contactInfo, currentInfo = contact });
        }

        /// <summary>
        /// Аутентификация.
        /// </summary>
        /// <param name="pass">Пароль для получения токена.</param>
        /// <returns>Токен для входа.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UnauthorizedResult), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(OkResult), StatusCodes.Status200OK)]
        public IActionResult Auth(string pass) => pass == _configuration.GetValue<string>("AuthenticationSettings:Password")
            ? Ok(_tokenBuilderService.GenerateToken())
            : Unauthorized();

        #region Private Methods

        /// <summary>
        /// Checks if contact is Active.
        /// </summary>
        /// <param name="key">Email or phone.</param>
        /// <param name="fullName">Full name of contact.</param>
        /// <returns>True if active</returns>
        private async Task<dynamic> CheckIsActive(string key, string fullName)
        {
            _logger.Info($"Check if {fullName} with {key} is active.");

            var findContact = await _dbContext.Contacts.FirstOrDefaultAsync(x => x.FullName == fullName
                && (x.Email.Equals(key) || x.PhoneNumber.Equals(key)));

            return findContact is null
                ? new { error = true, IsActive = false, message = "No such contact" }
                : new { error = false, IsActive = findContact.IsActive };
        }

        #endregion
    }
}
