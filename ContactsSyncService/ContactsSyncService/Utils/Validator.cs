using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Data;

namespace ContactsSyncService.Utils
{
    /// <summary>
    /// Отмечу, что вообще можно использовать паттерн проектирования под названием стратегия, но тут не так много правил проверки.
    /// </summary>
    public static class Validator
    {
        public enum Rule
        {
            NotNull,
            StringNotEmpty,
            NotNegative,
            NotPositive,
            NotZero
        }
        private delegate bool Checker(object obj);
        /// <summary>
        /// Алгоритм перебора всех объектов и применения к ним правила.
        /// </summary>
        /// <param name="checker">Правило.</param>
        /// <param name="values">Переменные для перебора.</param>
        /// <returns>True если валидно правилу, False если ошибка.</returns>
        private static dynamic ValidatorHelper(Checker checker, params object[] values)
        {
            var isActive = true;
            var errorList = new List<string>();

            foreach (var item in values)
            {
                isActive &= checker(item);

                if (!isActive)
                {
                    errorList.Add(nameof(item));
                }
            }

            return isActive ? new { IsActive = isActive } : new { IsActive = isActive, Errors = errorList.ToArray()};
        }

        /// <summary>
        /// Проверяет валидность правилу, которое указано.
        /// </summary>
        /// <param name="rule">Правило.</param>
        /// <param name="input">Переменные для перебора.</param>
        /// <returns>True если валидно правилу, False если ошибка.</returns>
        public static dynamic IsValid(Rule rule, params object[] input)
        {
            switch (rule)
            {
                case Rule.NotNull:
                    return ValidatorHelper(x => x is not null, input);
                case Rule.StringNotEmpty:
                    return ValidatorHelper(x => !string.IsNullOrEmpty(x as string), input);
                case Rule.NotNegative:
                    return ValidatorHelper(x => (int)x > 0, input);
                case Rule.NotPositive:
                    return ValidatorHelper(x => (int)x < 0, input);
                case Rule.NotZero:
                    return ValidatorHelper(x => (int)x != 0, input);
                default:
                    return false;
            }
        }
    }
}
