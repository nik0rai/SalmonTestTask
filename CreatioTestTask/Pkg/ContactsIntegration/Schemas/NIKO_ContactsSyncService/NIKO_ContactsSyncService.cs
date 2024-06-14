 namespace Terrasoft.Configuration
 {
 	using System;
	using System.Linq;
	using System.Dynamic;
	using System.Text;
    using System.ServiceModel.Activation;
    using System.ServiceModel;
    using System.Collections.Generic;
	using System.ServiceModel.Web;
    using Terrasoft.Common;
	using Terrasoft.Core;
    using Terrasoft.Core.DB;
    using Terrasoft.Web.Common;
	using Column = Core.DB.Column;
    using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using HttpContext = Web.Http.Abstractions.HttpContext;
    using global::Common.Logging;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;	
	
	/// <summary>
    /// Сервис для обновления платежной истории и контактной информации контакта.
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class NIKO_ContactsSyncService : BaseService
	{
		#region Fields: Private
			
		private static readonly ILog _log = LogManager.GetLogger("NIKO_ContactsSyncService");
		
		#endregion
		
		/// <summary>
		/// Проверка работы API.
		/// </summary>
		/// <returns>Объект ответа.</returns>
		[OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        public async Task<object> HealthStatus()
		{
			return await MakeAsyncGetRequest("/");
		}
		
		/// <summary>
		/// Получить инфу контакта и его адресов.
		/// </summary>
		/// <param name="contactId">Запрос.</param>
		/// <param name="key">Ключ.</param>
		/// <param name="fullName">ФИО.</param>
		/// <returns>Объект ответа.</returns>
		[OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        public async Task<Response> GetContactInfo(string id, string key, string fullName)
		{
			var result = new Response();
			var contactId = Guid.Parse(id);
			
			var token = await GetTokenAsync();
			var response = await MakeAsyncGetRequest($"/ContactsSync/GetContactInfo?key={key}&fullName={fullName}", token);	
			var deserealised = JsonConvert.DeserializeObject<dynamic>(response.result);
			
			var currentlyNotActive = UserNotActive(deserealised, contactId);
			
			if(currentlyNotActive.error) //No such contact
			{
				result.error = true;
				result.result = currentlyNotActive.result;
				
				return result;
			}
			else if (currentlyNotActive.result == "Contact is not active") 
			{
				result.result = currentlyNotActive.result;
				
				return result;
			}
			
			var responseHistory = await MakeAsyncGetRequest($"/ContactsSync/GetPaymentHistory?key={key}&fullName={fullName}", token);	
			var historyDeserealised = JsonConvert.DeserializeObject<dynamic>(responseHistory.result);
			
			var contactResult = UpdateContactGeneralInfo(deserealised.currentInfo, contactId);
			
			var addressesResult = InsertOrUpdateAddress(deserealised, contactId);
			
			var paymentsResult = InsertOrUpdatePayments(historyDeserealised, deserealised.currentInfo, contactId);
			
			var sb = new StringBuilder();
			sb.AppendLine($"Contact information: {contactResult.result}");
			sb.AppendLine($"Addresses information: {addressesResult.result}");
			sb.AppendLine($"Payments information: {paymentsResult.result}");

			result.result = sb.ToString();

			return result;
		}

		#region Get SysSettings values

		/// <summary>
		/// Получение URL ApiContactsInfo
		/// </summary>
		/// <returns>URL ApiContactsInfo</returns>
		private string GetUrlApiContactsInfo()
		{
			return Terrasoft.Core.Configuration.SysSettings.GetValue<string>(UserConnection, "NIKO_ApiContactsInfoUrl", "SysSettingsValue");
		}

		/// <summary>
		/// Получение пароля для получения токена.
		/// </summary>
		/// <returns>Пароль ApiContactsInfo.</returns>
		private string GetPassword()
		{
			return Terrasoft.Core.Configuration.SysSettings.GetValue<string>(UserConnection, "NIKO_ApiContactsPassword", "SysSettingsValue");
		}

		#endregion
		
		#region Aditional Helpers (translators)

		/// <summary>
		/// Получить id гендера.
		/// </summary>
		/// <param name="gender">Гендер true -> male, false -> female.</param>
		/// <returns>Guid male или female.</returns>
		public Guid GenderSetup(bool gender)
		{
			return gender ? new Guid("eeac42ee-65b6-df11-831a-001d60e938c6") : new Guid("fc2483f8-65b6-df11-831a-001d60e938c6");
		}

		/// <summary>
		/// Получить id типа адреса.
		/// </summary>
		/// <param name="addressType">Тип адреса true -> work, false -> home.</param>
		/// <returns>Guid work или home.</returns>
		public static Guid AddressTypeSetup(bool addressType)
		{
			return addressType ? new Guid("7d4cd953-688e-426f-a13a-2ed3fa75266c") : new Guid("7506f031-0769-4010-a12f-3ad557464e26");
		}
		
		/// <summary>
		/// Получить id города.
		/// </summary>
		/// <param name="city">Название города.</param>
		/// <returns>Id города.</returns>
		public static Guid CitySetup(string city, UserConnection UserConnection)
		{
			Select selectByCityName = new Select(UserConnection)
						.Column("Id")
						.From("City")
						.Where("Name").IsEqual(Column.Parameter(city)) as Select;
			
			var result = Guid.Empty;
			
			using (DBExecutor dbExecutor = UserConnection.EnsureDBConnection()) 
			{
				using (var reader = selectByCityName.ExecuteReader(dbExecutor)) 
				{
					while (reader.Read()) 
					{
						var cityId = reader.GetColumnValue<Guid>("Id");				
						result = cityId;
					}
				}
			}
				
			return result;
		}

		#endregion
		
		#region Web requests helper methods

		/// <summary>
		/// Создать POST запрос.
		/// </summary>
		/// <param name="url">Запрос.</param>
		/// <param name="token">Токен.</param>
		/// <param name="data">Body.</param>
		/// <returns>Объект ответа.</returns>
		private async Task<dynamic> MakeAsyncPostRequest(string url, string token = null, object data = null)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.BaseAddress = new Uri(GetUrlApiContactsInfo());

				if (token != null)
				{
					httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
				}

				HttpResponseMessage response;
				if (data != null)
				{
					response = await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
				}
				else
				{
					response = await httpClient.PostAsync(url, null);
				}

				var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

				if (!response.IsSuccessStatusCode)
				{
					return new { error = true, message = result };
				}

				return result;
			}
		}

		/// <summary>
		/// Создать GET запрос.
		/// </summary>
		/// <param name="url">Запрос.</param>
		/// <param name="token">Токен.</param>
		/// <returns>Объект ответа.</returns>
		private async Task<Response> MakeAsyncGetRequest(string url, string token = null)
		{
			using (var httpClient = new HttpClient())
			{
				var result = new Response();
				
				httpClient.BaseAddress = new Uri(GetUrlApiContactsInfo());

				if (token != null)
				{
					httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
				}

				var response = await httpClient.GetAsync(url);
				var content = await response.Content.ReadAsStringAsync();

				if (!response.IsSuccessStatusCode)
				{
					result.error = true;
				}
				
				result.result = content;
				
				return result;
			}
		}

		#endregion
	
		/// <summary>
		/// Получение токена.
		/// </summary>
		/// <returns>Токен</returns>
		private async Task<dynamic> GetTokenAsync()
		{
			return await MakeAsyncPostRequest($"/ContactsSync/Auth?pass={GetPassword()}");
		}

		/// <summary>
		/// Если пользователь не активен.
		/// </summary>
		/// <returns>Результат обновления.</returns>
		private Response UserNotActive(dynamic response, Guid contactId)
		{
			var result = new Response();
			bool errorParam = response.error; //(bool)(response.error as JValue).Value
			
			if (errorParam) //No such contact
			{
				result.error = true;
				result.result = response.message;
				
				return result;
			}
			try //Contact is not active
			{
				bool newStateParam = response.newState;
				if(!newStateParam)
				{
					var updateState = new Update(UserConnection, "Contact")
					.Set("NIKO_IsActive", Column.Const(false))
					.Where("Id").IsEqual(Column.Parameter(contactId)) as Update;
					var updateStateResult = updateState.Execute();
				
					result.error = false;
					result.result = response.message;
					
					return result; 
				}
			}
			catch
			{
				result.result = response.message;
				return result;
			}

			result.result = "Contact is active";

			return result;
		}

		/// <summary>
		/// Обновить основную инфу контакта.
		/// </summary>
		/// <returns>Результат обновления.</returns>
		private Response UpdateContactGeneralInfo(dynamic currentInfo, Guid contactId)
		{
			var result = new Response();
			try
			{
				var newGender = (Guid)GenderSetup((bool)currentInfo.gender);
				var newCity = (Guid)NIKO_ContactsSyncService.CitySetup((string)currentInfo.city, UserConnection);

				var updateCurrentContect = new Update(UserConnection, "Contact")
					.Set("Phone", Column.Parameter((string)currentInfo.phoneNumber))
					.Set("Email", Column.Parameter((string)currentInfo.email))
					.Set("CityId", Column.Parameter(newCity))
					.Set("NIKO_Home", Column.Parameter((string)currentInfo.home))
					.Set("NIKO_IsActive", Column.Parameter((bool)currentInfo.isActive))
					.Set("BirthDate", Column.Parameter((DateTime)currentInfo.dateOfBirth))
					.Set("GenderId", Column.Parameter(newGender))
					.Where("Id").IsEqual(Column.Parameter(contactId)) as Update;
					
				var updateResult = updateCurrentContect.Execute();
				
				result.error = updateResult < 1;
				result.result = updateResult > 0 ? "Contact info updated!" : "Contact info already actual.";
				return result;
			}
			catch (Exception ex)
			{
				result.error = true;
				result.result = ex.Message;
				
				return result;
			}
		}

		/// <summary>
		/// Обновить или добавить адреса контакта.
		/// </summary>
		/// <returns>Результат обновления/вставки.</returns>
		private Response InsertOrUpdateAddress(dynamic response, Guid contactId)
		{
			var result = new Response();
			try
			{
				List<ExpandoObject> iputData = JsonConvert.DeserializeObject<List<ExpandoObject>>(response.addContactInfo.ToString());

				var newAddressesList = new List<Address>();
				iputData.ForEach(obj =>
				{
					IDictionary<string, object> x = new Dictionary<string, object>(obj);
					newAddressesList.Add(new Address(new Guid((string)x["id"]), (string)x["phoneNumber"], 
					(string)x["email"], (string)x["city"], (string)x["home"], (bool)x["isActive"], (bool)x["addresType"], UserConnection));
				});
				
				string email = response.currentInfo.email;
				string phone = response.currentInfo.phoneNumber;

				var selectAddresses = new Select(UserConnection)
					.Column("Id")
					.Column("NIKO_Phone")
					.Column("NIKO_Email")
					.Column("NIKO_CityId")
					.Column("NIKO_Home")
					.Column("NIKO_IsActive")
					.Column("NIKO_AddressTypeId")
					.From("NIKO_ContactAddress")
					.Where("NIKO_ContactId").IsEqual(Column.Parameter(contactId))
					.And().OpenBlock("NIKO_Email").IsEqual(Column.Parameter(email))
						.Or("NIKO_Phone").IsEqual(Column.Parameter(phone))
					.CloseBlock() as Select;

				var addressesList = new List<Address>();
				using (var dbExecutor = UserConnection.EnsureDBConnection()) {
					using (var reader = selectAddresses.ExecuteReader(dbExecutor)) {
						while(reader.Read()) 
						{
							var Id = reader.GetColumnValue<Guid>("Id");
							var NIKO_Phone = reader.GetColumnValue<string>("NIKO_Phone");
							var NIKO_Email = reader.GetColumnValue<string>("NIKO_Email");
							var NIKO_CityId = reader.GetColumnValue<Guid>("NIKO_CityId");
							var NIKO_Home = reader.GetColumnValue<string>("NIKO_Home");
							var NIKO_IsActive = reader.GetColumnValue<bool>("NIKO_IsActive");
							var NIKO_AddressTypeId = reader.GetColumnValue<Guid>("NIKO_AddressTypeId");

							addressesList.Add(new Address((Guid)Id, (string)NIKO_Phone, (string)NIKO_Email,
							(Guid)NIKO_CityId, (string)NIKO_Home, (bool)NIKO_IsActive, (Guid)NIKO_AddressTypeId, UserConnection));
						}
					}
				}
				
				var toInsert = newAddressesList.Except(addressesList, new AddressComparer()).ToList();
				var toUpdate = newAddressesList.Intersect(addressesList, new AddressComparer()).Except(addressesList, new AddressFullComparer()).ToList();

				var taskResult = false;
				int inserted = 0;
				int updated = 0;
				
				toInsert.ForEach(x =>
				{
					string innerPhone = x.NIKO_Phone == null ? "" : x.NIKO_Phone;
					string innerEmail = x.NIKO_Email == null ? "" : x.NIKO_Email;
					
					var insert = new Insert(UserConnection)
						.Into("NIKO_ContactAddress")
						.Set("Id", Column.Parameter((Guid)x.Id))
						.Set("NIKO_ContactId", Column.Parameter((Guid)contactId))
						.Set("NIKO_Phone", Column.Parameter((string)innerPhone))
						.Set("NIKO_Email", Column.Parameter((string)innerEmail))
						.Set("NIKO_CityId", Column.Parameter((Guid)x.NIKO_CityId))
						.Set("NIKO_Home", Column.Parameter((string)x.NIKO_Home))
						.Set("NIKO_IsActive", Column.Parameter((bool)x.NIKO_IsActive))
						.Set("NIKO_AddressTypeId", Column.Parameter((Guid)x.NIKO_AddressTypeId)) as Insert;
						
					var insertedRows = insert.Execute();	
					inserted += insertedRows;
					taskResult |= insertedRows > 0;
				});

				toUpdate.ForEach(x =>
				{
					string innerPhone = x.NIKO_Phone == null ? "" : x.NIKO_Phone;
					string innerEmail = x.NIKO_Email == null ? "" : x.NIKO_Email;
					
					var update = new Update(UserConnection, "NIKO_ContactAddress")
						//.Set("Id", Column.Parameter((Guid)x.Id))
						.Set("NIKO_Phone", Column.Parameter((string)innerPhone))
						.Set("NIKO_Email", Column.Parameter((string)innerEmail))
						.Set("NIKO_CityId", Column.Parameter((Guid)x.NIKO_CityId))
						.Set("NIKO_Home", Column.Parameter((string)x.NIKO_Home))
						.Set("NIKO_IsActive", Column.Parameter((bool)x.NIKO_IsActive))
						.Set("NIKO_AddressTypeId", Column.Parameter((Guid)x.NIKO_AddressTypeId))
						.Where("NIKO_ContactId").IsEqual(Column.Parameter((Guid)contactId))
						.And().OpenBlock("NIKO_Email").IsEqual(Column.Parameter((string)email))
							.Or("NIKO_Phone").IsEqual(Column.Parameter((string)phone))
						.CloseBlock()
						.And("NIKO_Home").IsEqual(Column.Parameter((string)x.NIKO_Home))
						.And("NIKO_CityId").IsEqual(Column.Parameter((Guid)x.NIKO_CityId)) as Update;
					
					var updatedRows = update.Execute();
					updated += updatedRows;
					taskResult |= updatedRows > 0;
				});

				result.error = !taskResult;
				result.result = taskResult ? $"Sucessfully updated {updated} rows and added {inserted} rows" : "All data is up to date.";
				
				return result;
			}
			catch(Exception ex)
			{
				result.error = true;
				result.result = ex.Message;
				
				return result;
			}
			
			return result;
		}
		
		/// <summary>
		/// Обновить или добавить адреса платежи.
		/// </summary>
		/// <returns>Результат обновления/вставки.</returns>
		private Response InsertOrUpdatePayments(dynamic response, dynamic currentInfo, Guid contactId)
		{
			var result = new Response();
			try
			{
				List<ExpandoObject> iputData = JsonConvert.DeserializeObject<List<ExpandoObject>>(response.addHistory.ToString());

				var newPaymentsList = new List<Payments>();
				iputData.ForEach(obj =>
				{
					IDictionary<string, object> x = new Dictionary<string, object>(obj);
					newPaymentsList.Add(new Payments(new Guid((string)x["id"]), (string)x["phoneNumber"], 
					(string)x["email"], (string)x["paymentDetails"], (double)x["ammount"], (DateTime)x["paymentDateTime"]));
				});
				
				string email = currentInfo.email;
				string phone = currentInfo.phoneNumber;

				var selectPayments = new Select(UserConnection)
					.Column("Id")
					.Column("NIKO_Phone")
					.Column("NIKO_Email")
					.Column("NIKO_PaymentPurpose")
					.Column("NIKO_Sum")
					.Column("NIKO_PaymentDateTime")
					.From("NIKO_PaymentSchedule")
					.Where("NIKO_ContactId").IsEqual(Column.Parameter(contactId))
					.And().OpenBlock("NIKO_Email").IsEqual(Column.Parameter(email))
						.Or("NIKO_Phone").IsEqual(Column.Parameter(phone))
					.CloseBlock() as Select;

				var paymentsList = new List<Payments>();
				using (var dbExecutor = UserConnection.EnsureDBConnection()) {
					using (var reader = selectPayments.ExecuteReader(dbExecutor)) {
						while(reader.Read()) 
						{
							var Id = reader.GetColumnValue<Guid>("Id");
							var NIKO_Phone = reader.GetColumnValue<string>("NIKO_Phone");
							var NIKO_Email = reader.GetColumnValue<string>("NIKO_Email");
							var NIKO_PaymentPurpose = reader.GetColumnValue<string>("NIKO_PaymentPurpose");
							var NIKO_Sum = reader.GetColumnValue<double>("NIKO_Sum");
							var NIKO_PaymentDateTime = reader.GetColumnValue<DateTime>("NIKO_PaymentDateTime");

							paymentsList.Add(new Payments((Guid)Id, (string)NIKO_Phone, (string)NIKO_Email,
							(string)NIKO_PaymentPurpose, (double)NIKO_Sum, (DateTime)NIKO_PaymentDateTime));
						}
					}
				}
				
				var toInsert = newPaymentsList.Except(paymentsList, new PaymentsComparer()).ToList();
				var toUpdate = newPaymentsList.Intersect(paymentsList, new PaymentsComparer()).Except(paymentsList, new PaymentsFullComparer()).ToList();
				
				var taskResult = false;
				int inserted = 0;
				int updated = 0;
				
				toInsert.ForEach(x =>
				{
					string innerPhone = x.NIKO_Phone == null ? "" : x.NIKO_Phone;
					string innerEmail = x.NIKO_Email == null ? "" : x.NIKO_Email;
					
					var insert = new Insert(UserConnection)
						.Into("NIKO_PaymentSchedule")
						.Set("Id", Column.Parameter((Guid)x.Id))
						.Set("NIKO_ContactId", Column.Parameter((Guid)contactId))
						.Set("NIKO_Phone", Column.Parameter((string)innerPhone))
						.Set("NIKO_Email", Column.Parameter((string)innerEmail))
						.Set("NIKO_PaymentPurpose", Column.Parameter((string)x.NIKO_PaymentPurpose))
						.Set("NIKO_Sum", Column.Parameter((double)x.NIKO_Sum))
						.Set("NIKO_PaymentDateTime", Column.Parameter((DateTime)x.NIKO_PaymentDateTime)) as Insert;
						
					var insertedRows = insert.Execute();	
					inserted += insertedRows;
					taskResult |= insertedRows > 0;
				});

				toUpdate.ForEach(x =>
				{
					string innerPhone = x.NIKO_Phone == null ? "" : x.NIKO_Phone;
					string innerEmail = x.NIKO_Email == null ? "" : x.NIKO_Email;
					
					var update = new Update(UserConnection, "NIKO_PaymentSchedule")
						.Set("NIKO_Phone", Column.Parameter((string)innerPhone))
						.Set("NIKO_Email", Column.Parameter((string)innerEmail))
						.Set("NIKO_PaymentPurpose", Column.Parameter((string)x.NIKO_PaymentPurpose))
						.Set("NIKO_Sum", Column.Parameter((double)x.NIKO_Sum))
						.Set("NIKO_PaymentDateTime", Column.Parameter((DateTime)x.NIKO_PaymentDateTime))
						.Where("NIKO_ContactId").IsEqual(Column.Parameter((Guid)contactId))
						.And().OpenBlock("NIKO_Email").IsEqual(Column.Parameter((string)email))
							.Or("NIKO_Phone").IsEqual(Column.Parameter((string)phone))
						.CloseBlock()
						.And("NIKO_PaymentPurpose").IsEqual(Column.Parameter((string)x.NIKO_PaymentPurpose))
						.And("NIKO_PaymentDateTime").IsEqual(Column.Parameter((DateTime)x.NIKO_PaymentDateTime)) as Update;
					
					var updatedRows = update.Execute();
					updated += updatedRows;
					taskResult |= updatedRows > 0;
				});

				result.error = !taskResult;
				result.result = taskResult ? $"Sucessfully updated {updated} rows and added {inserted} rows" : "All data is up to date.";
				
				return result;
			}
			catch(Exception ex)
			{
				result.error = true;
				result.result = ex.Message;
				
				return result;
			}
			
			return result;
		}
		
		#region Classes
		
		public class Response 
		{
			public bool error { get; set; }
			public string result { get; set; }
			
			public Response()
			{
				error = false;
			}
		}

		public class Address
		{
			public Guid Id { get; set; }
			public string NIKO_Phone { get; set; }
			public string NIKO_Email { get; set; }
			public Guid NIKO_CityId { get; set; }
			public string NIKO_Home { get; set; }
			public bool NIKO_IsActive { get; set; }
			public Guid NIKO_AddressTypeId { get; set; }

			public Address(Guid id, string phone, string email, string city, string home, bool isActive, bool addressType, UserConnection UserConnection)
			{
				Id = id;
				NIKO_Phone = phone;
				NIKO_Email = email;
				NIKO_CityId = (Guid)NIKO_ContactsSyncService.CitySetup(city, UserConnection);
				NIKO_Home = home;
				NIKO_IsActive = isActive;
				NIKO_AddressTypeId = (Guid)NIKO_ContactsSyncService.AddressTypeSetup(addressType);
			}

			public Address(Guid id, string phone, string email, Guid city, string home, bool isActive, Guid addressType, UserConnection UserConnection) 
			{
				Id = id;
				NIKO_Phone = phone;
				NIKO_Email = email;
				NIKO_CityId = city;
				NIKO_Home = home;
				NIKO_IsActive = isActive;
				NIKO_AddressTypeId = addressType;
			}
			
			public override string ToString()
			{
				return $"({nameof(Id)}={Id}, {nameof(NIKO_Phone)}={NIKO_Phone}, {nameof(NIKO_Email)}={NIKO_Email}, {nameof(NIKO_CityId)}={NIKO_CityId}, {nameof(NIKO_Home)}={NIKO_Home}, " +
					$"{nameof(NIKO_IsActive)}={NIKO_IsActive}, {nameof(NIKO_AddressTypeId)}={NIKO_AddressTypeId})";
			}
		}
		
		#region Address comparers
		
		public class AddressComparer : IEqualityComparer<Address>
		{
			public bool Equals(Address a1, Address a2)
			{
				return a1.NIKO_Home == a2.NIKO_Home && a1.NIKO_CityId == a2.NIKO_CityId;
			}

			public int GetHashCode(Address obj)
			{
				return obj.NIKO_Home.GetHashCode() ^ obj.NIKO_CityId.GetHashCode();
			}
		}
		
		public class AddressFullComparer : IEqualityComparer<Address>
		{
			public bool Equals(Address a1, Address a2)
			{
				var a1Email = string.IsNullOrEmpty(a1.NIKO_Email) ? "" : (string)a1.NIKO_Email;
				var a2Email = string.IsNullOrEmpty(a2.NIKO_Email) ? "" : (string)a2.NIKO_Email;
				var a1Phone = string.IsNullOrEmpty(a1.NIKO_Phone) ? "" : (string)a1.NIKO_Phone;
				var a2Phone = string.IsNullOrEmpty(a2.NIKO_Phone) ? "" : (string)a2.NIKO_Phone;
				
				return a1Phone == a2Phone && a1Email == a2Email && a1.NIKO_CityId == a2.NIKO_CityId
					&& a1.NIKO_Home == a2.NIKO_Home && a1.NIKO_IsActive == a2.NIKO_IsActive && a1.NIKO_AddressTypeId == a2.NIKO_AddressTypeId;
			}

			public int GetHashCode(Address obj)
			{
				var phone = string.IsNullOrEmpty(obj.NIKO_Phone) ? "" : (string)obj.NIKO_Phone;
				var email = string.IsNullOrEmpty(obj.NIKO_Email) ? "" : (string)obj.NIKO_Email;
				var city = obj.NIKO_CityId == null ? Guid.Empty : obj.NIKO_CityId;
				var home = obj.NIKO_Home == null ? "" : obj.NIKO_Home;
				var isActive = obj.NIKO_IsActive == null ? false : obj.NIKO_IsActive;
				var addressType = obj.NIKO_AddressTypeId == null ? Guid.Empty : obj.NIKO_AddressTypeId;

				return phone.GetHashCode() ^ email.GetHashCode() ^ city.GetHashCode() ^ home.GetHashCode()
					^ isActive.GetHashCode() ^ addressType.GetHashCode();
			}
		}
		
		#endregion
		
		public class Payments
		{
			public Guid Id { get; set; }
			public string NIKO_Phone { get; set; }
			public string NIKO_Email { get; set; }
			public string NIKO_PaymentPurpose { get; set; }
			public double NIKO_Sum { get; set; }
			public DateTime NIKO_PaymentDateTime { get; set; }

			public Payments(Guid id, string phone, string email, string paymentsDetails, double ammount, DateTime paymentDateTime)
			{
				Id = id;
				NIKO_Phone = phone;
				NIKO_Email = email;
				NIKO_PaymentPurpose = paymentsDetails;
				NIKO_Sum = ammount;
				NIKO_PaymentDateTime = paymentDateTime;
			}
			
			public override string ToString()
			{
				return $"({nameof(Id)}={Id}, {nameof(NIKO_Phone)}={NIKO_Phone}, {nameof(NIKO_Email)}={NIKO_Email}, " +
					$"{nameof(NIKO_PaymentPurpose)}={NIKO_PaymentPurpose}, {nameof(NIKO_Sum)}={NIKO_Sum}, {nameof(NIKO_PaymentDateTime)}={NIKO_PaymentDateTime})";
			}
		}
		
		#region Payments comparers
		
		public class PaymentsComparer : IEqualityComparer<Payments>
		{
			public bool Equals(Payments a1, Payments a2)
			{
				return (string.IsNullOrEmpty(a1.NIKO_PaymentPurpose) ? "" : (string)a1.NIKO_PaymentPurpose) 
						== (string.IsNullOrEmpty(a2.NIKO_PaymentPurpose) ? "" : (string)a2.NIKO_PaymentPurpose) 
					&& a1.NIKO_PaymentDateTime == a2.NIKO_PaymentDateTime;
			}

			public int GetHashCode(Payments obj)
			{
				var paymentPurpose = string.IsNullOrEmpty(obj.NIKO_PaymentPurpose) ? "" : (string)obj.NIKO_PaymentPurpose;
				var paymentDateTime = obj.NIKO_PaymentDateTime == null ? default(DateTime) : (DateTime)obj.NIKO_PaymentDateTime;

				return paymentDateTime.GetHashCode() ^ paymentPurpose.GetHashCode();
			}
		}

		public class PaymentsFullComparer : IEqualityComparer<Payments>
		{
			public bool Equals(Payments a1, Payments a2)
			{
				return (string.IsNullOrEmpty(a1.NIKO_PaymentPurpose) ? "" : (string)a1.NIKO_PaymentPurpose) 
						== (string.IsNullOrEmpty(a2.NIKO_PaymentPurpose) ? "" : (string)a2.NIKO_PaymentPurpose) 
					&& a1.NIKO_Sum == a2.NIKO_Sum 
					&& a1.NIKO_PaymentDateTime == a2.NIKO_PaymentDateTime;
			}

			public int GetHashCode(Payments obj)
			{
				var paymentPurpose = string.IsNullOrEmpty(obj.NIKO_PaymentPurpose) ? "" : (string)obj.NIKO_PaymentPurpose;
				var sum = obj.NIKO_Sum == null ? default(double) : (double)obj.NIKO_Sum;
				var paymentDateTime = obj.NIKO_PaymentDateTime == null ? default(DateTime) : (DateTime)obj.NIKO_PaymentDateTime;

				return paymentPurpose.GetHashCode() ^ sum.GetHashCode() ^ paymentDateTime.GetHashCode();
			}
		}
		
		#endregion
		
		#endregion
	}
 }