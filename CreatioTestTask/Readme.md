- Для Creatio заведена база данных под названием creatio
- Для отдельного сервиса заведена база данных под названием postgres



        DB postgres

+-------------------------+
|        *Contacts*       |
+-------------------------+
|       Id (PK) - uuid    |
|    PhoneNumber - text   |
|      Email - text       |
|       City - text       |
|      Home - text        |
|    IsActive - boolean   |
|   DateOfBirth - date    |
|    FullName - text      |
|     Gender - boolean    | _female = F | male = T_
+-------------------------+


+-------------------------+
|        *Address*        |
+-------------------------+
|       Id (PK) - uuid    |
|    PhoneNumber - text   |
|      Email - text       |
|       City - text       |
|      Home - text        |
|    IsActive - boolean   |
|   AddresType - boolean  | _home = F | work = T_
+-------------------------+


+-----------------------------+
|          *Payments*         |
+-----------------------------+
|         Id (PK) - uuid      |
|      PhoneNumber - text     |
|        Email - text         |
|    PaymentDetails - text    |
|        Ammount - money      |
| PaymentDateTime - timestamp |
+-----------------------------+

Для того, чтобы установить нужно запустить скрипты step-1, а потом step-2.
На step-2 происходит восстановление БД и создание перечисленных таблиц выше