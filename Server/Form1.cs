﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapper;
using ModelsLibrary;

namespace Server
{
    public partial class Form1: Form
    {
        MyRequest clientMessage = null;
        MyRequest serverMessage = null;
        TcpListener server = null;
        BinaryFormatter bf1 = new BinaryFormatter();

        public Form1()
        {
            
            InitializeComponent();
        }

        private async void Server()
        {
            try
            {
                

                server = new TcpListener(IPAddress.Parse(IPField.Text), int.Parse(PortField.Text));
                server.Start(10);
                Console.WriteLine("Server Started!");

                while (true)
                {

                    TcpClient acceptor = server.AcceptTcpClient();
                    NetworkStream ns = acceptor.GetStream();
                    

                    clientMessage = (MyRequest)bf1.Deserialize(ns);

                    if (clientMessage.type == "Login")
                    {
                        using (var db = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=Users"))
                        {
                            
                            var response = db.QueryFirstOrDefault<Users>($"Select * from Users where loginU=={clientMessage.login} and PasswordU == {clientMessage.password}");
                            serverMessage.type = "login-result";
                            serverMessage.message = $"success: {response.ToString()}";
                            

                        }
                    }



                    bf1.Serialize(ns,serverMessage);
                  
                    
                    
                    ns.Close();
                    acceptor.Close();

                    


                }

            }
            catch (SocketException sockEx)
            {
                Console.WriteLine($"\n>server socket error! :{sockEx}");
            }
            catch (Exception Ex)
            {
                Console.WriteLine($"\n> runtime error! :{Ex}");
            }
            finally
            {
                if (server != null)
                {
                    server.Stop();

                }
                Console.WriteLine("Server Stopped!");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
