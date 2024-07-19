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
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["myservicedb"].ConnectionString);
            sqlConnection.Open();

            string query1 = "select distinct orders.order_id, emp.first_name + ' ' + emp.last_name, emp.phone_number as employee_number, cl.first_name + ' ' + cl.last_name, cl.phone_number, orders.description, orders.status, orders.order_date, sum(serv.cost) over(partition by orders.order_id) from employees emp join services_rendered serv_rend on (emp.employee_id = serv_rend.employee_id) join services serv on (serv_rend.service_id = serv.service_id) join orders on (serv_rend.order_id = orders.order_id) join clients cl on (cl.client_id = orders.client_id);";
            string query2 = "select orders.order_id, serv.service_name, serv.cost from services_rendered serv_rend join services serv on (serv_rend.service_id = serv.service_id) join orders on (serv_rend.order_id = orders.order_id) join clients cl  on (cl.client_id = orders.client_id) order by orders.order_id;";

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
