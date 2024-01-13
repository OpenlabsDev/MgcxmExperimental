using Openlabs.Mgcxm.Internal;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace Openlabs.Mgcxm.GUI
{
    internal sealed class MgcxmOverviewGui : GuiPanel
    {
        private FrameView _httpRequestPanel;
        private TextView _httpRequestText;
        private FrameView _webSocketPanel;
        private TextView _webSocketText;
        private FrameView _internalLogPanel;
        private TextView _internalLogText;

        private async Task RapidUpdate()
        {
            for (; ; )
            {
                Application.MainLoop.Invoke(() =>
                {
                    _httpRequestPanel.Title = string.Format("HTTP Request Panel [{0} server(s) running]", Mgcxm.Net.MgcxmHttpListener.ServerCount);
                    _webSocketPanel.Title = string.Format("WebSocket Panel [{0} server(s) running]", Mgcxm.Net.MgcxmSocketListener.ServerCount);
                });

                await Task.Delay(500);
            }
        }

        private void OnLogMessageMade(LogMessage lMsg)
        {
            _internalLogText.Text = string.Format("{0}\n", lMsg.nonAnsiMessage) + _internalLogText.Text;
            UpdateDisplays();
        }

        private void UpdateDisplays()
        {
            Application.MainLoop.Invoke(() =>
            {
                _httpRequestText.Move(_httpRequestText.Text.Length, 0);
                _webSocketText.Move(_webSocketText.Text.Length, 0);
                _internalLogText.Move(_internalLogText.Text.Length, 0);

                _httpRequestPanel.SetNeedsDisplay();
                _webSocketPanel.SetNeedsDisplay();
                _internalLogPanel.SetNeedsDisplay();
            });
        }

        protected override void InitialDraw()
        {
            Logger.Initialize(OnLogMessageMade);
            Task.Factory.StartNew(RapidUpdate, TaskCreationOptions.LongRunning);

            Console.Title = string.Format("{0} (Mgcxm Application)", Constants.ApplicationName);
            Title = string.Format("Mgcxm Overview Panel ({0})", Constants.Version);

            Events.OnHttpRequestMade += (httpServer, httpRequest) =>
            {
                _httpRequestText.Text = string.Format("[{0}] {1}:{2}\n", 
                    httpServer.GetType().Name,
                    httpRequest.HttpMethod,
                    httpRequest.Uri) + _httpRequestText.Text;
                UpdateDisplays();
            };

            Events.OnWsRequestMade += (wsServer, wsRoute) =>
            {
                _webSocketText.Text = string.Format("[{0}] {1}:{2}\n",
                    wsServer.GetType().Name,
                    wsRoute.RouteId,
                    wsRoute.Route) + _webSocketText.Text;
                UpdateDisplays();
            };

            var topPanelLeft = _httpRequestPanel = new FrameView("HTTP Request Panel")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Percent(40)
            };

            var topPanelRight = _webSocketPanel = new FrameView("WebSocket Panel")
            {
                X = Pos.Right(topPanelLeft),
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Percent(40)
            };

            var bottomPanel = _internalLogPanel = new FrameView("Internal Log Panel")
            {
                X = 0,
                Y = Pos.Bottom(topPanelLeft),
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var topPanelLeftText = _httpRequestText = new TextView()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                ColorScheme = Colors.Menu
            };

            var topPanelRightText = _webSocketText = new TextView()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                ColorScheme = Colors.Menu
            };

            var bottomPanelText = _internalLogText = new TextView()
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
                ColorScheme = Colors.Menu
            };

            topPanelLeft.Add(topPanelLeftText);
            topPanelRight.Add(topPanelRightText);
            bottomPanel.Add(bottomPanelText);

            Add(topPanelLeft, topPanelRight, bottomPanel);
        }
    }
}
