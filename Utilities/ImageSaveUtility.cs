using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace app.Models
{

    public class ImageSaveUtility
    {

    
        public void SerializeAndSave(string name)
        {
            try
            {
                // instantiate a MemoryStream and a new instance of our class          
                MemoryStream ms = new MemoryStream();
                ClassToSerialize c = new ClassToSerialize(name);
                // create a new BinaryFormatter instance
                BinaryFormatter b = new BinaryFormatter();
                // serialize the class into the MemoryStream
                b.Serialize(ms, c);
                ms.Seek(0, 0);

                // Show the information
                Console.WriteLine("Ms Length: " + ms.Length.ToString());
                int res = SaveToDB(name, ms.ToArray());
                Console.WriteLine("nDB RetVal: " + res.ToString());
                //Clean up
                ms.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void RetrieveAndDeserialize(string userName)
        {
            MemoryStream ms2 = new MemoryStream();
            byte[] buf = RetrieveFromDB(userName);
            ms2.Write(buf, 0, buf.Length);
            ms2.Seek(0, 0);
            BinaryFormatter b = new BinaryFormatter();
            ClassToSerialize c = (ClassToSerialize)b.Deserialize(ms2);
            Console.WriteLine("Deserialized Name: " + c.name);

            Console.WriteLine("Portion of Deserialized float array");
            for (int j = 0; j < 100; j++)
            {
                Console.WriteLine( c.fltArray[j].ToString());
            }
            ms2.Close();
        }
        private int SaveToDB(string imgName, byte[] imgbin)
        {
            var connection = new SqlConnection("Server=(local);DataBase=Northwind;User Id=sa;Password=;");

            SqlCommand command = new SqlCommand("INSERT INTO Employees (firstname,lastname,photo) VALUES(@img_name, @img_name, @img_data)", connection);
            // (need to write something to first and lastname columns
            // because of constraints)
            SqlParameter param0 = new SqlParameter("@img_name", SqlDbType.VarChar, 50)
            {
                Value = imgName
            };
            command.Parameters.Add(param0);
            SqlParameter param1 = new SqlParameter("@img_data", SqlDbType.Image, 50);
            param1.Value = imgbin;
            command.Parameters.Add(param1);
            connection.Open();
            int numRowsAffected = command.ExecuteNonQuery();
            connection.Close();
            return numRowsAffected;
        }
        private byte[] RetrieveFromDB(string lastname)
        {
            SqlConnection connection = new SqlConnection("Server=(local);DataBase=Northwind; User Id=sa;Password=;"); SqlCommand command = new SqlCommand("select top 1 Photo from Employees where lastname = '" + lastname + "'", connection);

            connection.Open();
            SqlDataReader dr = command.ExecuteReader();
            dr.Read();
            byte[] imgData = (byte[])dr["Photo"];
            connection.Close();
            return imgData;
        }
    }


    [Serializable]
    public class ClassToSerialize
    {
        public string name;
        public float[] fltArray;
        // constructor initializes name and creates the sample array of floats
        public ClassToSerialize(string theName)
        {
            this.name = theName;
            float[] theArray = new float[1000];
            Random rnd = new System.Random();
            for (int i = 0; i < 1000; i++)
                theArray[i] = (float)rnd.NextDouble() * 1000;
            fltArray = theArray;
        }
    }


}
