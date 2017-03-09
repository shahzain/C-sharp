using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocksMechanism
{


    class Program
    {
        private static SerialPort myport;
        private static bool first_time;
        private static DateTime start;

        static void Main(string[] args)
        {
            Boolean loop = true;
                    

            String Stop = "stop",
                   Farward="farward",
                   back="back",
                   urgent_stop="urgent stop",
                   left="left",
                   right = "right";

            int count = 1;

            while (loop)
            {
                Send("go");
                Send("slow");
             //  Send(back);
              //  Send(left);
              //  Send(right);
                count++;

                if(count== 100000000/4 || count==100000000/3 || count==100000000 / 2)
                {
                    urgent_Send(urgent_stop);
                }

                if (count > 1000000000)
                {
                    loop = false;
                }
            }

        }

        private static void Send(string S)
        {

            if (first_time==true)
            {
                start = DateTime.Now;
                first_time = false;
                goto first;
            }

            var end = DateTime.Now;

            var elapsed = (end - start - TimeSpan.FromSeconds(0)).Seconds;

            

            if ((int)elapsed > 3)
            {
                myport = new SerialPort();
                myport.BaudRate = 9600;

                myport.PortName = "COM9";
                myport.StopBits = StopBits.One;

                try
                {
                    myport.Open();
                    //  Thread.Sleep(2000);

                    myport.Write(S);
                    myport.Close();


                }
                catch (Exception ex)
                {
                    Console.Out.Write(ex + "\n");
                }
                 Console.Out.Write(S+"\n");
                start = DateTime.Now;
            }


            first:
            ((Action)(() => { }))();
        }





        public static void urgent_Send(string S)
        {
           
                Console.Out.Write(S + "\n");
            
        }


    }
}
