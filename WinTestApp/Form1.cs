using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using HtmlAgilityPack;
using Sekmen.StaticSiteGenerator;

namespace WinTestApp;

public partial class Form1 : Form
{
    private static readonly HttpClient HttpClientInstance = new();

    public Form1()
    {
        InitializeComponent();

        // Redirect Console.Out (used by Logger.Info and Logger.Error) to txtLog
        Console.SetOut(new ControlWriter(txtLog));

        InitializeDefaults();
    }

    private void InitializeDefaults()
    {
        txtSiteUrl.Text = "https://localhost:5001/";
        txtTargetUrl.Text = "https://static.mysite.com/";
        txtOutputFolder.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "StaticSiteExport");
        txtAdditionalUrls.Text = "/about" + Environment.NewLine + "/contact" + Environment.NewLine + "/privacy";

        // Add sample string replacement row
        dgvReplacements.Rows.Add("http://localhost:5000", "https://static.mysite.com");
    }

    private async void btnExport_Click(object sender, EventArgs e)
    {
        string siteUrl = txtSiteUrl.Text.Trim();
        string targetUrl = txtTargetUrl.Text.Trim();
        string outputFolder = txtOutputFolder.Text.Trim();

        if (string.IsNullOrWhiteSpace(siteUrl))
        {
            MessageBox.Show("Please enter a valid Source Site URL.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(outputFolder))
        {
            MessageBox.Show("Please select an Output Directory.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string[] additionalUrls = txtAdditionalUrls.Text
            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(u => u.Trim())
            .Where(u => !string.IsNullOrEmpty(u))
            .ToArray();

        List<StringReplacements> replacements = [];
        foreach (DataGridViewRow row in dgvReplacements.Rows)
        {
            if (row.IsNewRow) continue;
            string? oldVal = row.Cells[0].Value?.ToString();
            string? newVal = row.Cells[1].Value?.ToString();
            if (!string.IsNullOrEmpty(oldVal))
            {
                replacements.Add(new StringReplacements(oldVal, newVal ?? string.Empty));
            }
        }

        ExportCommand command = new(
            SiteUrl: siteUrl,
            AdditionalUrls: additionalUrls,
            TargetUrl: string.IsNullOrEmpty(targetUrl) ? "/" : targetUrl,
            OutputFolder: outputFolder,
            StringReplacements: replacements.ToArray()
        );

        btnExport.Enabled = false;
        progressBar1.Visible = true;
        Logger.Info($"=== Starting Site Export for {command.SiteUrl} ===");
        Logger.Info($"Target URL: {command.TargetUrl}");
        Logger.Info($"Output Folder: {command.OutputFolder}");

        try
        {
            await Generator.ExportWebsite(HttpClientInstance, command);
            Logger.Info("=== Export completed successfully! ===");
        }
        catch (Exception ex)
        {
            Logger.Error("Export failed with an error", ex);
            MessageBox.Show($"Export error: {ex.Message}", "Export Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnExport.Enabled = true;
            progressBar1.Visible = false;
        }
    }

    private void btnBrowseFolder_Click(object sender, EventArgs e)
    {
        using FolderBrowserDialog dialog = new();
        dialog.Description = "Select Destination Output Directory";
        dialog.UseDescriptionForTitle = true;
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtOutputFolder.Text = dialog.SelectedPath;
        }
    }

    private void btnAddReplacement_Click(object sender, EventArgs e)
    {
        dgvReplacements.Rows.Add("", "");
    }

    private void btnRemoveReplacement_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in dgvReplacements.SelectedRows)
        {
            if (!row.IsNewRow)
            {
                dgvReplacements.Rows.Remove(row);
            }
        }
    }

    private void btnOpenFolder_Click(object sender, EventArgs e)
    {
        string path = txtOutputFolder.Text.Trim();
        if (Directory.Exists(path))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        else
        {
            MessageBox.Show($"Directory standard path does not exist yet:\n{path}", "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void btnClearLog_Click(object sender, EventArgs e)
    {
        txtLog.Clear();
    }

    // ----------------------------------------------------
    // Feature Tester Event Handlers
    // ----------------------------------------------------

    private void btnRunValidationTest_Click(object sender, EventArgs e)
    {
        string inputUrl = txtValidationInputUrl.Text.Trim();
        string baseUriStr = txtValidationBaseUri.Text.Trim();

        StringBuilder sb = new();
        sb.AppendLine($"--- Testing UrlValidation for: '{inputUrl}' ---");
        sb.AppendLine($"NormalizeSourceUrl('{inputUrl}') => {UrlValidation.NormalizeSourceUrl(inputUrl)}");
        sb.AppendLine($"IsResourceFile('{inputUrl}')      => {UrlValidation.IsResourceFile(inputUrl)}");
        sb.AppendLine($"IsInternalLink('{inputUrl}')      => {UrlValidation.IsInternalLink(inputUrl)}");

        if (Uri.TryCreate(baseUriStr, UriKind.Absolute, out Uri? baseUri))
        {
            sb.AppendLine($"IsValidResourceUrl('{inputUrl}', {baseUri}) => {UrlValidation.IsValidResourceUrl(inputUrl, baseUri)}");
        }
        else
        {
            sb.AppendLine($"[!] Invalid Base URI: {baseUriStr}");
        }

        txtValidationOutput.Text = sb.ToString();
    }

    private void btnRunRewriterTest_Click(object sender, EventArgs e)
    {
        string sourceUrl = txtRewriterSource.Text.Trim();
        string targetUrl = txtRewriterTarget.Text.Trim();
        string html = txtHtmlInput.Text;

        ExportCommand command = new(
            SiteUrl: sourceUrl,
            AdditionalUrls: Array.Empty<string>(),
            TargetUrl: targetUrl,
            OutputFolder: "C:\\Temp",
            StringReplacements: Array.Empty<StringReplacements>()
        );

        try
        {
            string rewritten = UrlHelpers.UpdateHtmlUrls(html, sourceUrl, command);
            txtHtmlOutput.Text = rewritten;
        }
        catch (Exception ex)
        {
            txtHtmlOutput.Text = $"Error rewriting HTML: {ex.Message}";
        }
    }

    private void btnRunExtractorTest_Click(object sender, EventArgs e)
    {
        string html = txtExtractorHtml.Text;
        string baseUriStr = txtExtractorBase.Text.Trim();

        if (!Uri.TryCreate(baseUriStr, UriKind.Absolute, out Uri? baseUri))
        {
            txtExtractorOutput.Text = $"[!] Invalid Base URI: {baseUriStr}";
            return;
        }

        try
        {
            HtmlAgilityPack.HtmlDocument doc = new();
            doc.LoadHtml(html);

            HashSet<string> resources = Extractor.ExtractResourceUrls(doc, baseUri);

            StringBuilder sb = new();
            sb.AppendLine($"Found {resources.Count} static resource URLs:");
            foreach (string res in resources)
            {
                sb.AppendLine($" - {res}");
            }

            txtExtractorOutput.Text = sb.ToString();
        }
        catch (Exception ex)
        {
            txtExtractorOutput.Text = $"Error extracting resources: {ex.Message}";
        }
    }
}