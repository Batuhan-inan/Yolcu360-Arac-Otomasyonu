using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arac_Kiralama
{
    internal class keyhandler
    {
        public class CustomKeyboardHandler : IKeyboardHandler
        {
            public bool OnPreKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
            {
                // Tuşa basıldığı anı (RawKeyDown veya KeyDown) yakalıyoruz ⌨️
                if (type == KeyType.RawKeyDown || type == KeyType.KeyDown)
                {
                    // F12 tuşunun sanal tuş kodu (Virtual Key Code) 123'tür (Keys.F12) 🔍
                    if (windowsKeyCode == (int)Keys.F12)
                    {
                        // DevTools ekranını açıyoruz
                        chromiumWebBrowser.ShowDevTools();
                        return true; // Tuş olayını işlediğimizi CefSharp'a bildiriyoruz
                    }
                }
                return false;
            }

            public bool OnKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
            {
                return false;
            }
        }
    }
}
