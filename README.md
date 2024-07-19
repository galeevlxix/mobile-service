### Описание
Проектирование, программирование и администрирование базы данных в MS SQL Server и разработка интерфейса в Windows Forms для запроса данных для «Мобил-Сервис».
### Оглавление
1. Создание базы данных servicedb1  
2. Создание и определение таблиц  
3. Вводим ограничение для проверки цены на любую услугу  
4. Добавим необходимые связи между таблицами c каскадным изменением зависимых строк в подчиненной таблице при изменении строк в родительской  
5. Построение реляционной модели  
6. Введем данные в таблицу  
7. Запрос на SQL для вывода необходимых данных из таблиц query1  
8. Запрос на SQL для вывода необходимых данных из таблиц query2  
9. Листинг программы на С# для работы с базой данных  
10. Результат работы программы  
### 1. Создание базы данных «Мобил-Сервис» servicedb1  
![image](https://github.com/user-attachments/assets/e62ba5c9-6f7e-4bf5-a49b-fb85b1fc34db)  
### 2. Создание и определение таблиц
Заказы  
![image](https://github.com/user-attachments/assets/57324441-e9de-4354-a1f0-b1b19f0cd0fd)  
Клиенты  
![image](https://github.com/user-attachments/assets/b42a9908-f458-4010-aa7a-8b267fc72bb1)  
Оказанные услуги  
![image](https://github.com/user-attachments/assets/ea524212-0c27-4a2b-a2d3-239509010011)  
Сотрудники  
![image](https://github.com/user-attachments/assets/cb40e586-5c4a-436c-84d8-f6251600a54d)  
Услуги сервисного центра  
![image](https://github.com/user-attachments/assets/9379e9ac-7dd6-47da-85d7-28ee598fe9d5)  
### 3. Вводим ограничение для проверки цены на любую услугу
цена не меньше 100р:  
```sql
constraint [cost_check] check (cost > 100)
```
### 4. Добавим необходимые связи между таблицами c каскадным изменением зависимых строк в подчиненной таблице при  изменении строк в родительской  
связь 1:М между подчиненной таблицей services_rendered и родительской таблицей orders
```sql
constraint [sr_order_fk] foreign key ([order_id]) references [dbo].[orders] ([order_id]) on update cascade
```
связь 1:М между подчиненной таблицей services_rendered и родительской таблицей employees
```sql
constraint [sr_employee_fk] foreign key ([employee_id]) references [dbo].[employees] ([employee_id]) on update cascade
```
связь 1:М между подчиненной таблицей services_rendered и родительской таблицей services
```sql
constraint [sr_service_fk] foreign key ([service_id]) references [dbo].[services] ([service_id]) on update cascade
```
связь 1:М между подчиненной таблицей orders и родительской таблицей clients
```sql
constraint [o_client_fk] foreign key ([client_id]) references [dbo].[clients] ([client_id]) on update cascade
```
### 5. Построение реляционной модели
![image](https://github.com/user-attachments/assets/266225fc-4a6f-4bc3-a0b1-dded72412f9d)  
### 6. Введем данные в таблицу
Заказы  
![image](https://github.com/user-attachments/assets/316f094b-b7eb-432a-8e5f-0c84b9d67314)  
Клиенты  
![image](https://github.com/user-attachments/assets/7d129b8b-9e63-4ceb-9639-ae7ac88c78fc)  
Оказанные услуги  
![image](https://github.com/user-attachments/assets/0e877b9d-c2dc-48ae-94fd-f990682ca1ce)  
Сотрудники  
![image](https://github.com/user-attachments/assets/0f45ecfd-d45c-4772-9603-740c4f57d5ee)  
Услуги сервисного центра  
![image](https://github.com/user-attachments/assets/9d2dbee5-9c46-4007-9eb6-c8cfede4ae5b)  
### 7. Запрос на SQL для вывода необходимых данных из таблиц  
query1:  
```sql
select distinct
orders.order_id,
emp.first_name + ' ' + emp.last_name as "employee name",
emp.phone_number as "employee number",
cl.first_name + ' ' + cl.last_name as "client name",
cl.phone_number as "client number",
orders.description as trouble,
orders.status as status,
orders.order_date as date,
sum(serv.cost) over (partition by orders.order_id) as sum
from employees emp join services_rendered serv_rend
on (emp.employee_id = serv_rend.employee_id)
join services serv
on (serv_rend.service_id = serv.service_id)
join orders
on (serv_rend.order_id = orders.order_id)
join clients cl 
on (cl.client_id = orders.client_id);
 ```
![image](https://github.com/user-attachments/assets/f2c02730-b25e-4323-8f5e-e04c527a44c5)  
### 8. Запрос на SQL для вывода необходимых данных из таблиц 
query2:
```sql
select 
orders.order_id as order_id,
serv.service_name as service,
serv.cost as cost
from services_rendered serv_rend
join services serv
on (serv_rend.service_id = serv.service_id)
join orders
on (serv_rend.order_id = orders.order_id)
join clients cl 
on (cl.client_id = orders.client_id)
order by orders.order_id;
```
![image](https://github.com/user-attachments/assets/2112ce77-e27d-4b22-be47-b0e393d29b28)  
### 9. Листинг программы на С# для работы с базой данных
```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace reginapractice2
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;

        private List<string[]> data;
        private List<string[]> data1;
        int ord_num;
        string t = null;
        string t1 = null;
        string t2 = null;
        SqlDataReader reader;
        SqlDataReader reader1;

        public Form1()
        {
            data = new List<string[]>();
            data1 = new List<string[]>();
            ord_num = 1;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new 
SqlConnection(ConfigurationManager.ConnectionStrings["myservicedb"].ConnectionString);
            sqlConnection.Open();

            string query1 = "select distinct orders.order_id, emp.first_name + ' ' + emp.last_name, emp.phone_number as 
            employee_number, cl.first_name + ' ' + cl.last_name, cl.phone_number, orders.description, orders.status, 
            orders.order_date, sum(serv.cost) over(partition by orders.order_id) from employees emp join services_rendered 
            serv_rend on (emp.employee_id = serv_rend.employee_id) join services serv on (serv_rend.service_id = serv.service_id) 
            join orders on (serv_rend.order_id = orders.order_id) join clients cl on (cl.client_id = orders.client_id);";
            string query2 = "select orders.order_id, serv.service_name, serv.cost from services_rendered serv_rend join 
            services serv on (serv_rend.service_id = serv.service_id) join orders on (serv_rend.order_id = orders.order_id) join clients 
            cl  on (cl.client_id = orders.client_id) order by orders.order_id;";

            SqlCommand command1 = new SqlCommand(query1, sqlConnection);
            SqlCommand command2 = new SqlCommand(query2, sqlConnection);

            reader = command1.ExecuteReader();
            while (reader.Read())
            {
                data.Add(new string[9]);

                data[data.Count - 1][0] = reader[0].ToString().Trim();
                data[data.Count - 1][1] = reader[1].ToString().Trim();
                data[data.Count - 1][2] = reader[2].ToString().Trim();
                data[data.Count - 1][3] = reader[3].ToString().Trim();
                data[data.Count - 1][4] = reader[4].ToString().Trim();
                data[data.Count - 1][5] = reader[5].ToString().Trim();
                data[data.Count - 1][6] = reader[6].ToString().Trim();
                data[data.Count - 1][7] = reader[7].ToString().Trim();
                data[data.Count - 1][8] = reader[8].ToString().Trim();
            }

            reader.Close();

            reader1 = command2.ExecuteReader();
            while (reader1.Read())
            {
                data1.Add(new string[3]);

                data1[data1.Count - 1][0] = reader1[0].ToString().Trim();
                data1[data1.Count - 1][1] = reader1[1].ToString().Trim();
                data1[data1.Count - 1][2] = reader1[2].ToString().Trim();
            }
            reader1.Close();

            sqlConnection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            for (int i = 0; i < data.Count(); i++)
            {
                if (Int32.Parse(data[i][0]) == ord_num)
                {
                    t = "Номер заказа: " + data[i][0];
                    listView1.Items.Add(t);
                    t = "Клиент: " + data[i][3] + "    |   Номер телефона клиента - " + data[i][4];
                    listView1.Items.Add(t);
                    t = "Мастер: " + data[i][1] + "    |   Номер телефона мастера - " + data[i][2];
                    listView1.Items.Add(t);
                    t = "Проблема: " + data[i][5];
                    listView1.Items.Add(t);
                    t = "Статус: " + data[i][6];
                    listView1.Items.Add(t);
                    t = "Дата заказа: " + data[i][7];
                    listView1.Items.Add(t);
                    t1 = "Услуги: ";
                    for (int j = 0; j < data1.Count(); j++)
                    {
                        if (Int32.Parse(data[i][0]) == Int32.Parse(data1[j][0]))
                        {
                            t1 += data1[j][1] + " (" + data1[j][2] + "р)    ";
                        }
                    }
                    listView1.Items.Add(t1.Trim());
                    t = "Сумма к оплате: " + data[i][8];
                    listView1.Items.Add(t);
                }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            ord_num = Int32.Parse(numericUpDown1.Value.ToString());
        }
    }
}
```
### 10. Результат работы программы  
![image](https://github.com/user-attachments/assets/2af23ca3-da33-4b5c-ac45-9365e8ec3a89)  
![image](https://github.com/user-attachments/assets/36ac4204-4118-496d-80bd-d7d4e40c6c68)  
![image](https://github.com/user-attachments/assets/322e4fd1-5e88-424d-ac11-7efe11650c02)  

