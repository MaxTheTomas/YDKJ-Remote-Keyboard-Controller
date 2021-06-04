using System;
using System.Diagnostics;
using Gma.System.MouseKeyHook;
using WebSocketSharp;
using WebSocketSharp.Server;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using WebSocket = WebSocketSharp.WebSocket;

namespace KeyboardController {
    public class HostWebSocket {
        private WebSocketServer server;
        public HostWebSocket() {
            server = new WebSocketServer(8001); 
            server.AddWebSocketService<Receiver>("/");
            server.Start();
        }
        
        
        public void Close() {
            server.Stop();
            server.RemoveWebSocketService("/");
            server = null;
        }
        
        class Receiver : WebSocketBehavior {
            [DllImport ("User32.dll")]
            static extern int SetForegroundWindow(IntPtr point);

            protected override void OnMessage(MessageEventArgs e) {
                SendKeys.SendWait(e.Data);
            }
        }
    }
}