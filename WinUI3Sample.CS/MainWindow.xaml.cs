// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUI3Sample.CS
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // Use IDataTransferManagerInterop to get DataTransferManager.
        // Reference: https://learn.microsoft.com/en-us/windows/apps/develop/ui-input/display-ui-objects#for-classes-that-implement-idatatransfermanagerinterop
        [System.Runtime.InteropServices.ComImport]
        [System.Runtime.InteropServices.Guid("3A3DCD6C-3EAB-43DC-BCDE-45671CE800C8")]
        [System.Runtime.InteropServices.InterfaceType(
            System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown)]
        interface IDataTransferManagerInterop
        {
            IntPtr GetForWindow([System.Runtime.InteropServices.In] IntPtr appWindow,
                [System.Runtime.InteropServices.In] ref Guid riid);
            void ShowShareUIForWindow(IntPtr appWindow);
        }
        static readonly Guid _dtm_iid = new Guid(0xa5caee9b, 0x8708, 0x49d1, 0x8d, 0x36, 0x67, 0xd2, 0x5a, 0x8d, 0xa0, 0x0c);

        private readonly DataTransferManager _dataTransferManager;

        public MainWindow()
        {
            this.InitializeComponent();

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var interop = Windows.ApplicationModel.DataTransfer.DataTransferManager.As<IDataTransferManagerInterop>();
            IntPtr result = interop.GetForWindow(hWnd, _dtm_iid);
            _dataTransferManager = WinRT.MarshalInterface<Windows.ApplicationModel.DataTransfer.DataTransferManager>.FromAbi(result);
            _dataTransferManager.ShareProvidersRequested += OnShareProvidersRequested;
        }
        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var interop = Windows.ApplicationModel.DataTransfer.DataTransferManager.As<IDataTransferManagerInterop>();
            interop.ShowShareUIForWindow(hWnd);
        }

        private void OnShareProvidersRequested(DataTransferManager sender, ShareProvidersRequestedEventArgs args)
        {
            // Create 
            string copyToClipboardTitle = "Copy";

            var copyToClipboardIcon = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets//Square44x44Logo.targetsize-24_altform-unplated.png"));

            var uiSettings = new UISettings();
            var accentColor = uiSettings.GetColorValue(UIColorType.Accent);

            var copyToClipboardShareProvider = new ShareProvider(copyToClipboardTitle, copyToClipboardIcon, accentColor, CopyToClipboardProviderHandler);
            args.Providers.Add(copyToClipboardShareProvider);
        }

        private void CopyToClipboardProviderHandler(ShareProviderOperation operation)
        {
            // Create a dataPackage with only a copied text, if you remove the SetText, the exception remains the same.
            var dataPackage = new DataPackage();
            dataPackage.SetText("Copied text");

            Clipboard.SetContent(dataPackage);
            operation?.ReportCompleted();
        }
    }
}
