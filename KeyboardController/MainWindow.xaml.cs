using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Gma.System.MouseKeyHook;
using WebSocketSharp;
using WebSocket = WebSocketSharp.WebSocket;

namespace KeyboardController {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            IKeyboardMouseEvents m = Hook.GlobalEvents();
            m.KeyDown += (o, a) => { if (a.KeyCode == Keys.Home) Environment.Exit(0); };
            KeyDown += (o, a) => {
                if (client != null && client.ReadyState == WebSocketState.Open) {
                    if (a.Key != Key.Home) {
                        if (a.Key.ToString().Length == 1) {
                            client.Send(a.Key.ToString());
                        }
                        else if (a.Key.ToString().Length == 2 && a.Key.ToString().StartsWith("D")) {
                            client.Send(a.Key.ToString().Replace("D", ""));
                        }
                        else if (a.Key == Key.Enter || a.Key == Key.Return)
                            client.Send("{Enter}");
                        else if (a.Key == Key.Back)
                            client.Send("{Backspace}");
                        else if (a.Key == Key.Space)
                            client.Send(" ");
                        else {
                            client.Send("{" + a.Key + "}");
                        }
                    }
                }
            };
        }

        private HostWebSocket host;

        public void StartServer(object sender, RoutedEventArgs routedEventArgs) {
            host = new HostWebSocket();
            StartS.IsEnabled = false;
            StartC.IsEnabled = false;
            StopC.IsEnabled = false;
        }

        public void StopServer(object sender, RoutedEventArgs routedEventArgs) {
            if (host != null) host.Close();
            host = null;
            
            StartS.IsEnabled = true;
            StartC.IsEnabled = true;
            StopC.IsEnabled = true;
        }

        private WebSocket client;

        public void StartClient(object sender, RoutedEventArgs routedEventArgs) {
            StartC.IsEnabled = false;
            StartS.IsEnabled = false;
            StopS.IsEnabled = false;
            IpBox.IsEnabled = false;
            
            try {
                client = new WebSocket($"ws://{IpBox.Text}:8001/");
                client.OnClose += (s, a) => StopClient(null, null); 
                client.Connect();
            } catch (Exception e) {
                Error.Content = e.Message;
            }
        }

        public void StopClient(object sender, RoutedEventArgs routedEventArgs) {
            if (client != null) {
                if (client.IsAlive)
                    client.Close();
                client = null;
            }
            
            IpBox.IsEnabled = true;
            StartC.IsEnabled = true;
            StartS.IsEnabled = true;
            StopS.IsEnabled = true;
        }
    }
}
