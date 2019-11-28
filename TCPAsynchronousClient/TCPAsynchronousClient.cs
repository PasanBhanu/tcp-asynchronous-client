using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCPAsynchronousClient
{
    public class AsynchronousClient
    {
        // *** Event Handlers *** //

        /// <summary>
        /// Notify the Recieved Data
        /// </summary>
        /// <param name="data">Recieved Data</param>
        public delegate void DataReceivedEventHandler(String data);
        public event DataReceivedEventHandler OnDataRecievedEvent;

        /// <summary>
        /// Notify the Connection Status of Socket
        /// </summary>
        /// <param name="status">Connection Status</param>
        public delegate void OnConnectEventHandler(bool status);
        public event OnConnectEventHandler OnConnectEvent;

        // *** Properties *** //

        // Connection Parameters
        private IPAddress ipAddress;
        private int port;

        // Socket Parameters
        private Socket socket;
        private byte[] readerBuffer = new byte[256];

        // *** Methods *** //

        /// <summary>
        /// Create a TCP Asynchronous Client. This client is connect to the server and port with passed parameters.
        /// </summary>
        /// <param name="_ip">Server IP</param>
        /// <param name="_port">Server Port</param>
        public AsynchronousClient(string _ip, int _port)
        {
            ipAddress = IPAddress.Parse(_ip);
            port = _port;
        }

        /// <summary>
        /// Connect to the server
        /// </summary>
        public void Connect()
        {
            try
            {
                // Close the socket if open
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    System.Threading.Thread.Sleep(10);
                    socket.Close();
                }

                // Create the socket object
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Define the Server address and port
                IPEndPoint epServer = new IPEndPoint(ipAddress, port);

                // Connect to server non-Blocking method
                socket.Blocking = false;
                AsyncCallback onconnect = new AsyncCallback(OnConnect);
                socket.BeginConnect(epServer, onconnect, socket);
            }
            catch (Exception ex)
            {
                OnConnectEvent(false);
                throw new Exception("Socket Connection Falied. Message : " + ex.ToString());
            }
        }

        /// <summary>
        /// Check connection status of the socket
        /// </summary>
        /// <returns>True or False based on status</returns>
        public bool IsConnected()
        {
            if (socket != null)
            {
                return socket.Connected;
            }
            return false;
        }

        // Setup Callbacks if Socket is Connected
        private void OnConnect(IAsyncResult ar)
        {
            Socket _socket = (Socket)ar.AsyncState;

            try
            {
                if (_socket.Connected)
                {
                    SetupRecieveCallback(_socket);
                    OnConnectEvent(true);
                }
                else
                {
                    OnConnectEvent(false);
                    throw new Exception("Cannot Establish the Socket Connection");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Setup Recieve Callback for Async Listning
        private void SetupRecieveCallback(Socket _socket)
        {
            try
            {
                AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                _socket.BeginReceive(readerBuffer, 0, readerBuffer.Length, SocketFlags.None, recieveData, _socket);
            }
            catch (Exception ex)
            {
                Dispose();
                throw new Exception("Recieve Callback Setup Failed");
            }
        }

        // Recieve data from TCP
        private void OnRecievedData(IAsyncResult ar)
        {
            Socket _socket = (Socket)ar.AsyncState;

            if (IsConnected())
            {
                try
                {
                    // Check data is available
                    int nBytesRec = _socket.EndReceive(ar);
                    if (nBytesRec > 0)
                    {
                        string sRecieved = "";
                        for (int i = 0; i < nBytesRec; i++)
                        {
                            sRecieved += (char)readerBuffer[i];
                        }

                        // Fire Data Recieved Event
                        OnDataRecievedEvent(sRecieved);

                        // If the Connection is Still Usable Restablish the Callback
                        SetupRecieveCallback(_socket);
                    }
                    else
                    {
                        Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Dispose();
                    throw new Exception("Recieve Operation Failed");
                }
            }
        }

        /// <summary>
        /// Write Data to Socket
        /// </summary>
        /// <param name="_data">Data to be written</param>
        /// <returns>Success status as Boolean Value</returns>
        public bool Write(String _data)
        {
            // Check Connection
            if (IsConnected())
            {
                try
                {
                    Byte[] byteDateLine = Encoding.ASCII.GetBytes(_data.ToCharArray());
                    socket.Send(byteDateLine, byteDateLine.Length, 0);
                    return true;
                }
                catch (Exception ex)
                {
                    Dispose();
                    throw new Exception("Data Writing Operation Failed");
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Write Data to Socket
        /// </summary>
        /// <param name="_data">Data to be written</param>
        /// <returns>Success status as Boolean Value</returns>
        public bool Write(byte[] _data)
        {
            // Check Connection
            if (IsConnected())
            {
                try
                {
                    socket.Send(_data, _data.Length, 0);
                    return true;
                }
                catch (Exception ex)
                {
                    Dispose();
                    throw new Exception("Data Writing Operation Failed");
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Close the Socket Connection
        /// </summary>
        public void Dispose()
        {
            if (socket != null && socket.Connected)
            {
                OnConnectEvent(false);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }

        /// <summary>
        /// Disconnect the socket
        /// </summary>
        public void Disconnect()
        {
            if (socket != null && socket.Connected)
            {
                OnConnectEvent(false);
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
    }
}
