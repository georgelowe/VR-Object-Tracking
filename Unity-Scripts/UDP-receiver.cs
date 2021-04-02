using System.Dynamic;
using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.UI;

// This class is responsible for receiving and handling the UDP data sent from the object tracking script
public class UDP_Receiver : MonoBehaviour {

    Thread receiveThread;
    UdpClient client;

    // This text field is in the debug window, it updates the user with real time UDP information
    [SerializeField]
    public Text udpTextField;

    public int port = 8051;
    public string latestPacket="";
    private Vector3 myVec = new Vector3(0,-1,0);
    float x, z, height;

    private static void Main()
    {
       UDP_Receiver receiveObj=new UDP_Receiver();
       receiveObj.init();

        string text="";
        do
        {
             text = Console.ReadLine();
        }
        while(!text.Equals("exit"));
    }

    public void Start()
    {
        udpTextField.text = "Start";

        // Get the current y position of the ball as this changes when the surface is re-aligned
        height = transform.localPosition.y;
        init();
    }

   // Called multiple times per frame -> the position of the ball is updated, as is the debug information
    void OnGUI()
    {

        // UDP debug text field is updated with latest packet information
        string packet_str = myVec.ToString();
        string message_str = "Receiving UDP Packets"+ "\nPort: " + port + "\nLatest Packet: \n"+ packet_str;
        udpTextField.text = message_str;

        // The ball position is updated
        transform.localPosition = myVec;
    }

    // Thread is initialised
    private void init()
    {
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    // UDP client is initialised and the data sent from the object tracking script is interpreted and converted into usable form
    private  void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                latestPacket=text;

                // The UDP data is received as a string, so it is split into the seperate X and Z components
                string[] coords = text.Split(',');

                    x = float.Parse(coords[0]);
                    z = float.Parse(coords[1]);

                    // If the ball is out of bounds, it is hidden from the display. Moving it to -1, -1, -1 is more efficient than de-activation
                    if(x == 0 && z == 0){
                        myVec = new Vector3(-1,-1,-1);
                    }
                    else{

                        // The coordinates are recalcuated to account for local position of the ball once the surface is moved
                        x = x*-1;
                        z = z-0.3f;

                        Vector3 v3 = new Vector3(x,height,z);

                        // This is the final vector that will be used in the OnGUI() method to move the ball
                        myVec = v3;
                    }
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }
}
