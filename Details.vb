Imports System.Web
Imports CMS_API.WebUI.WebControls
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.ClientScriptManager


Namespace ExternalExaminers
    Public Class Details

        Inherits BasePlaceHolderControl

        Dim externalexaminersdetails As New Div
        Dim recordPanel As New Panel

        Dim examinerid As String
        Dim title As String
        Dim firstname As String
        Dim lastname As String
        Dim faculty As String
        Dim employer As String
        Dim appointstartdate As String
        Dim appointenddate As String
        Dim awardboard As String

        Private request As HttpRequest
        Private response As HttpResponse

        Public Sub addToOutput(ByVal controlIn As Object)
            Me.Controls.Add(controlIn)
        End Sub

        Private Sub ExternalExaminersFormDetails_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            Dim myDataReader As SqlDataReader = Nothing
            Try
                request = HttpContext.Current.Request
                response = HttpContext.Current.Response

                recordPanel.ID = "recordpanel"
                addToOutput(recordPanel)
                Me.FindControl("recordpanel").Visible = True

                externalexaminersdetails.CssClass = "externalexaminersdetails"
                recordPanel.Controls.Add(externalexaminersdetails)

                examinerid = "123456789" 'request.QueryString("examinerid")


                Dim name As String = ""
                Dim email As String = ""
                Dim appointdate As String = ""
                Dim enddate As String = ""
                Dim employer As String = ""
                Dim faculty As String = ""
                Dim type As String = ""
                Dim awardboard As String = ""

                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                myDatabaseUtil.addParameter("@examinerid", examinerid)
                myDataReader = myDatabaseUtil.getSqlDataReader("storproc_viewexaminer")

                If myDataReader.HasRows Then
                    While myDataReader.Read()

                        name = myDataReader("title") + " " + myDataReader("first_name") + " " + myDataReader("last_name")

                        If Not myDataReader("email") Is DBNull.Value Then
                            email = myDataReader("email")
                        End If

                        If Not myDataReader("appoint_date") Is DBNull.Value Then
                            appointdate = myDataReader("appoint_date")
                        End If

                        If Not myDataReader("end_date") Is DBNull.Value Then
                            enddate = myDataReader("end_date")
                        End If

                        If Not myDataReader("employer") Is DBNull.Value Then
                            employer = myDataReader("employer")
                        End If

                        If Not myDataReader("faculty") Is DBNull.Value Then
                            faculty = myDataReader("faculty")
                        End If

                        If Not myDataReader("notes") Is DBNull.Value Then
                            type = myDataReader("notes")
                        End If

                        If Not myDataReader("award_board") Is DBNull.Value Then
                            awardboard = myDataReader("award_board")
                        End If


                        Dim TextBox1 As New TextBox()
                        TextBox1.Text = name
                        TextBox1.ID = "name"
                        TextBox1.ValidationGroup = "formgroup"
                        TextBox1.Enabled = False

                        Dim Label1 As New Label
                        Label1.Text = "<strong>Examiner name:</strong>"
                        Label1.AssociatedControlID = TextBox1.ID

                        Dim Div1 As New Div()
                        Div1.Controls.Add(Label1)
                        Div1.Controls.Add(TextBox1)
                        Div1.Attributes.Add("style", "clear:both")

                        Dim TextBox2 As New TextBox()
                        TextBox2.Text = email
                        TextBox2.ID = "email"
                        TextBox2.ValidationGroup = "formgroup"
                        TextBox2.Enabled = False

                        Dim Label2 As New Label
                        Label2.Text = "<strong>Email:</strong>"
                        Label2.AssociatedControlID = TextBox2.ID

                        Dim Div2 As New Div()
                        Div2.Controls.Add(Label2)
                        Div2.Controls.Add(TextBox2)
                        Div2.Attributes.Add("style", "clear:both")

                        Dim TextBox3 As New TextBox()
                        TextBox3.Text = FormatDateTime(appointdate, DateFormat.LongDate)
                        TextBox3.ID = "appointdate"
                        TextBox3.ValidationGroup = "formgroup"
                        TextBox3.Enabled = False

                        Dim Label3 As New Label
                        Label3.Text = "<strong>Examiner appointdate:</strong>"
                        Label3.AssociatedControlID = TextBox3.ID

                        Dim Div3 As New Div()
                        Div3.Controls.Add(Label3)
                        Div3.Controls.Add(TextBox3)
                        Div3.Attributes.Add("style", "clear:both")

                        Dim TextBox4 As New TextBox()
                        TextBox4.Text = enddate
                        TextBox4.ID = "enddate"
                        TextBox4.ValidationGroup = "formgroup"
                        TextBox4.Enabled = False

                        Dim Label4 As New Label
                        Label4.Text = "<strong>Examiner enddate:</strong>"
                        Label4.AssociatedControlID = TextBox4.ID

                        Dim Div4 As New Div()
                        Div4.Controls.Add(Label4)
                        Div4.Controls.Add(TextBox4)
                        Div4.Attributes.Add("style", "clear:both")

                        Dim TextBox5 As New TextBox()
                        TextBox5.Text = employer
                        TextBox5.ID = "employer"
                        TextBox5.ValidationGroup = "formgroup"
                        TextBox5.Enabled = False

                        Dim Label5 As New Label
                        Label5.Text = "<strong>Examiner employer:</strong>"
                        Label5.AssociatedControlID = TextBox5.ID

                        Dim Div5 As New Div()
                        Div5.Controls.Add(Label5)
                        Div5.Controls.Add(TextBox5)
                        Div5.Attributes.Add("style", "clear:both")

                        Dim TextBox6 As New TextBox()
                        TextBox6.Text = faculty
                        TextBox6.ID = "faculty"
                        TextBox6.ValidationGroup = "formgroup"
                        TextBox6.Enabled = False

                        Dim Label6 As New Label
                        Label6.Text = "<strong>Examiner faculty:</strong>"
                        Label6.AssociatedControlID = TextBox6.ID

                        Dim Div6 As New Div()
                        Div6.Controls.Add(Label6)
                        Div6.Controls.Add(TextBox6)
                        Div6.Attributes.Add("style", "clear:both")

                        Dim TextBox7 As New TextBox()
                        TextBox7.Text = type
                        TextBox7.ID = "type"
                        TextBox7.ValidationGroup = "formgroup"
                        TextBox7.Enabled = False

                        Dim Label7 As New Label
                        Label7.Text = "<strong>Examiner type:</strong>"
                        Label7.AssociatedControlID = TextBox7.ID

                        Dim Div7 As New Div()
                        Div7.Controls.Add(Label7)
                        Div7.Controls.Add(TextBox7)
                        Div7.Attributes.Add("style", "clear:both")

                        externalexaminersdetails.Controls.Add(Div1)
                        'externalexaminersdetails.Controls.Add(Div2)
                        externalexaminersdetails.Controls.Add(Div3)
                        ' externalexaminersdetails.Controls.Add(Div4)
                        externalexaminersdetails.Controls.Add(Div5)
                        externalexaminersdetails.Controls.Add(Div6)
                        ' externalexaminersdetails.Controls.Add(Div7)

                    End While
                Else
                    addToOutput(New P("No examiner found. "))
                End If

            Catch ex As Exception
                response.Write("ERROR - ExternalExaminersFormDetails_Init: " & ex.Message())
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try

        End Sub
    End Class
End Namespace

