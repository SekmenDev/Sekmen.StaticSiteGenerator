using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WinTestApp;

/// <summary>
/// A custom TextWriter that redirects Console output to a WinForms TextBox control thread-safely.
/// </summary>
public class ControlWriter : TextWriter
{
    private readonly TextBox _textbox;

    public ControlWriter(TextBox textbox)
    {
        _textbox = textbox;
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        AppendText(value.ToString());
    }

    public override void Write(string? value)
    {
        if (value != null)
        {
            AppendText(value);
        }
    }

    public override void WriteLine(string? value)
    {
        if (value != null)
        {
            AppendText(value + Environment.NewLine);
        }
        else
        {
            AppendText(Environment.NewLine);
        }
    }

    private void AppendText(string text)
    {
        if (_textbox.IsDisposed) return;

        if (_textbox.InvokeRequired)
        {
            try
            {
                _textbox.BeginInvoke(new Action(() => AppendText(text)));
            }
            catch (ObjectDisposedException)
            {
                // Form/TextBox was closed/disposed
            }
        }
        else
        {
            _textbox.AppendText(text);
        }
    }
}
