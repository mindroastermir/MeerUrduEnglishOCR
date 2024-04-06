using Google.Cloud.Vision.V1;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MeerUrduEnglishOCR.Dialogs
{
  public partial class OcrDialog : Window, IComponentConnector
  {
    // private bool _contentLoaded;

    public OcrDialog() => InitializeComponent();

    private void btnOpenFile_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return;
      foreach (string fileName in openFileDialog.FileNames)
      {
        txtFilePath.Text = fileName;
        imgPage.Source = (System.Windows.Media.ImageSource) new BitmapImage(new Uri(fileName));
      }
    }

    private void butOcrStart_Click(object Sender, RoutedEventArgs e)
    {
      doOcr(this.txtFilePath.Text);
      checkReqCounter();
    }

    private void doOcr(string fileName)
    {
      if (txtFilePath.Text == "" || txtJsonFile.Text == "")
      {
        System.Windows.MessageBox.Show("Image File or Json Key Missing. Please Try again");
      }
        else
        {
                // doSinglePageOCR();
                string text = txtFilePath.Text;
                imgPage.Source = new BitmapImage(new Uri(text));
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", txtJsonFile.Text);
                ImageAnnotatorClient imageAnnotatorClient = ImageAnnotatorClient.Create();
                Google.Cloud.Vision.V1.Image image = Google.Cloud.Vision.V1.Image.FromFile(text);
                try
                {
                    TextAnnotation textAnnotation = imageAnnotatorClient.DetectDocumentText(image);
                    setTotalRequests();
                    rtbUrduOcrText.Document.Blocks.Clear();
                    rtbUrduOcrText.Document.Blocks.Add((System.Windows.Documents.Block)new System.Windows.Documents.Paragraph((Inline)new Run(textAnnotation.Text)));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        
    }

    private void btnClear_Click(object sender, RoutedEventArgs e)
    {
      rtbUrduOcrText.Document.Blocks.Clear();
    }

    private void MyOcrTest_Loaded(object sender, RoutedEventArgs e)
    {
      checkReqCounter();
      string path = "10000300.ocr";
      if (File.Exists(path))
      {
        SQLiteConnection connection = new SQLiteConnection("Data Source=" + path + ";Version=3;New=False;Compress=True;Password=P@kistanUrdu");
        connection.Open();
        string commandText = "SELECT * FROM Content";
        DataSet dataSet = new DataSet();
        new SQLiteDataAdapter(commandText, connection).Fill(dataSet);
        dgBookContent.ItemsSource = (IEnumerable) dataSet.Tables[0].DefaultView;
      }
      else
      {
        SQLiteConnection connection = new SQLiteConnection("Data Source=" + path + ";Version=3;New=False;Compress=True;Password=P@kistanUrdu");
        connection.Open();
        new SQLiteCommand("create table Content (pageNo integer, Content string)", connection).ExecuteNonQuery();
      }
    }

    private void setTotalRequests()
    {
      SQLiteConnection connection = new SQLiteConnection("Data Source=" + "10000999.meer" + ";Version=3;New=False;Compress=True;Password=P@kistanUrdu");
      connection.Open();
      SQLiteDataReader sqLiteDataReader = new SQLiteCommand("Select * from ReqCounter", connection).ExecuteReader();
      if (sqLiteDataReader.Read())
      {
        int num1 = (int) sqLiteDataReader["counter"];
        sqLiteDataReader.Close();
        sqLiteDataReader.Dispose();
        int num2 = num1 + 1;
        new SQLiteCommand("UPDATE ReqCounter SET counter = " + (object) num2 + " WHERE id = 1", connection).ExecuteNonQuery();
        tbTotReqNo.Text = num2.ToString();
        connection.Close();
        if (num2 < 1000)
          tbRemainingRequestsNo.Text = (1000 - num2).ToString();
        if (num2 != 900)
          return;
        tbTotReqNo.Background = (Brush) Brushes.Yellow;
        tbTotReqNo.Foreground = (Brush) Brushes.Red;
        int num3 = (int) System.Windows.MessageBox.Show("Only 100 Free Requests Lef.");
      }
      else
      {
        int num = (int) System.Windows.MessageBox.Show("No Record Found!");
        connection.Close();
      }
    }

    private void txtFileIdNo_LostFocus(object sender, RoutedEventArgs e)
    {
      if (this.txtEndPageNo.Text == "" || txtStPageNo.Text == "" || this.txtFileIdNo.Text == "")
        return;
      int num = int.Parse(txtEndPageNo.Text) - int.Parse(this.txtStPageNo.Text);
      this.txtFileIdNoEnd.Text = (int.Parse(this.txtFileIdNo.Text) + num).ToString();
    }

    private void btnSetTotRequests_Click(object sender, RoutedEventArgs e)
    {
      string text = this.txtSetTotalRequests.Text;
      SQLiteConnection connection = new SQLiteConnection("Data Source=" + "10000999.meer" + ";Version=3;New=False;Compress=True;Password=mjbx_P@ssw0rd");
      connection.Open();
      new SQLiteCommand("UPDATE ReqCounter SET counter = " + text + " WHERE id = 1", connection).ExecuteNonQuery();
      connection.Close();
    }

    private void btnOpenJson_Click(object sender, RoutedEventArgs e)
    {
      OpenFileDialog openFileDialog = new OpenFileDialog();
      if (openFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return;
      foreach (string fileName in openFileDialog.FileNames)
        this.txtJsonFile.Text = fileName;
    }

    private void checkReqCounter()
    {
      string str = "10000999.meer";
      if (!File.Exists("10000999.meer"))
      {
        int num = 10;
        SQLiteConnection connection = new SQLiteConnection("Data Source=" + str + ";Version=3;New=False;Compress=True;Password=P@kistanUrdu");
        connection.Open();
        new SQLiteCommand("Create Table ReqCounter (id int unique, counter int, comment string)", connection).ExecuteNonQuery();
        new SQLiteCommand("Insert into ReqCounter (id, counter, comment) values (1, " + (object) num + ", 'how many requests this month')", connection).ExecuteNonQuery();
        new SQLiteCommand("Select * from ReqCounter", connection).ExecuteReader().Read();
        connection.Close();
      }
      else
        setTotalRequests();
    }

    private void doSinglePageOCR()
    {
      string text = txtFilePath.Text;
      imgPage.Source = new BitmapImage(new Uri(text));
      Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", txtJsonFile.Text);
      ImageAnnotatorClient imageAnnotatorClient = ImageAnnotatorClient.Create();
      Google.Cloud.Vision.V1.Image image = Google.Cloud.Vision.V1.Image.FromFile(text);
      try
      {
        TextAnnotation textAnnotation = imageAnnotatorClient.DetectDocumentText(image);
        setTotalRequests();
        rtbUrduOcrText.Document.Blocks.Clear();
        rtbUrduOcrText.Document.Blocks.Add((System.Windows.Documents.Block) new System.Windows.Documents.Paragraph((Inline) new Run(textAnnotation.Text)));
      }
      catch (Exception ex)
      {
        System.Windows.MessageBox.Show(ex.Message);
      }
    }

  }
}
