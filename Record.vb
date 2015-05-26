Imports System.Web
Imports CMS_API.WebUI.WebControls
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.ClientScriptManager


Namespace ExternalExaminers

    Public Class Record

        Inherits BasePlaceHolderControl

        Dim updateButton As Button
        Dim editLink As LinkButton
        Dim recordPanel As New Panel
        Dim awardUpdatePanel As New Panel
        Dim awardUpdateCompletePanel As New Panel
        Dim eeFieldset As New FieldSet
        Dim examinerid As String
        Dim awardboard As String = ""
        Dim PortalUser

        Dim externalexaminersdetails As New Div

        Private request As HttpRequest
        Private response As HttpResponse

        Public Sub addToOutput(ByVal controlIn As Object)
            Me.Controls.Add(controlIn)
        End Sub

        Private Sub ExternalExaminersRecord_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            request = HttpContext.Current.Request
            response = HttpContext.Current.Response

            eeFieldset.ID = "reportfieldset"
            addToOutput(eeFieldset)

            recordPanel.ID = "recordpanel"
            eeFieldset.Controls.Add(recordPanel)
            Me.FindControl("recordpanel").Visible = True

            awardUpdatePanel.ID = "awardupdatepanel"
            recordPanel.Controls.Add(awardUpdatePanel)
            Me.FindControl("awardupdatepanel").Visible = False

            awardUpdateCompletePanel.ID = "awardupdatecompletepanel"
            recordPanel.Controls.Add(awardUpdateCompletePanel)
            Me.FindControl("awardupdatecompletepanel").Visible = False

            externalexaminersdetails.CssClass = "externalexaminersdetails"
            recordPanel.Controls.Add(externalexaminersdetails)

            examinerid = request.QueryString("id")

            createRecord(examinerid, True, "form")

            'editLink = New LinkButton
            'editLink.ID = "editlink"
            'editLink.Text = "Edit"
            'AddHandler editLink.Click, AddressOf editLink_Click

            ' recordPanel.Controls.Add(New Span("Award Board: " + awardboard + " "))
            '  recordPanel.Controls.Add(editLink)

    

            awardUpdatePanel.Attributes.Add("style", "padding:10px; width:350px; border:1px solid #ccc; background-color: #ebebeb")

            Me.FindControl("awardupdatepanel").Visible = False
            Me.FindControl("awardupdatecompletepanel").Visible = False

        End Sub

        Public Sub New()
            request = HttpContext.Current.Request
            response = HttpContext.Current.Response
        End Sub

        Public Function createRecord(ByVal examinerid As String, ByVal withAwardBoard As Boolean, ByVal version As String)

            Dim name As String = ""
            Dim email As String = ""
            Dim appointdate As String = ""
            Dim enddate As String = ""
            Dim employer As String = ""
            Dim faculty As String = ""
            Dim type As String = ""
            Dim myDataReader As SqlDataReader = Nothing
            Try

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
                            appointdate = appointdate.Substring(0, appointdate.Length - 8)
                        End If

                        If Not myDataReader("end_date") Is DBNull.Value Then
                            enddate = myDataReader("end_date")
                            enddate = enddate.Substring(0, enddate.Length - 8)
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

                        If version = "word" Then


                            externalexaminersdetails.Controls.Add(New P("Name: " + name))
                            externalexaminersdetails.Controls.Add(New P("Email: " + email))
                            externalexaminersdetails.Controls.Add(New P("Date of appointment: " + FormatDateTime(appointdate, DateFormat.LongDate)))
                            externalexaminersdetails.Controls.Add(New P("Employer: " + employer))

                            If type = "External Examiner (Award)" Then
                                externalexaminersdetails.Controls.Add(New P("Award board: " + awardboard))
                            End If

                        Else

                            Dim TextBox1 As New TextBox()
                            TextBox1.Text = name
                            TextBox1.ID = "name"
                            TextBox1.ValidationGroup = "formgroup"
                            TextBox1.Enabled = False

                            Dim Label1 As New Label
                            Label1.Text = "Name of examiner:"
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
                            Label2.Text = "Email:"
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
                            Label3.Text = "Date of initial appointment as an external examiner:"
                            Label3.AssociatedControlID = TextBox3.ID

                            Dim Div3 As New Div()
                            Div3.Controls.Add(Label3)
                            Div3.Controls.Add(TextBox3)
                            Div3.Attributes.Add("style", "clear:both")

                            Dim TextBox4 As New TextBox()
                            TextBox4.Text = FormatDateTime(enddate, DateFormat.LongDate)
                            TextBox4.ID = "enddate"
                            TextBox4.ValidationGroup = "formgroup"
                            TextBox4.Enabled = False

                            Dim Label4 As New Label
                            Label4.Text = "Examiner end date:"
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
                            Label5.Text = "Home institution:"
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
                            Label6.Text = "Examiner faculty:"
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
                            Label7.Text = "Examiner type:"
                            Label7.AssociatedControlID = TextBox7.ID

                            Dim Div7 As New Div()
                            Div7.Controls.Add(Label7)
                            Div7.Controls.Add(TextBox7)
                            Div7.Attributes.Add("style", "clear:both")

                            'Dim awardboardTextBox As New TextBox
                            'awardboardTextBox.ValidationGroup = "updateexaminergroup"
                            'awardboardTextBox.ID = "awardboard"
                            'awardboardTextBox.Text = awardboard

                            'Dim awardboardLabel As New Label
                            'awardboardLabel.Text = "Award board:"
                            'awardboardLabel.AssociatedControlID = awardboardTextBox.ID

                            'Dim awardboardDiv1 As New Div()
                            'awardboardDiv1.Controls.Add(awardboardLabel)
                            'awardboardDiv1.Controls.Add(awardboardTextBox)
                            'awardboardDiv1.Attributes.Add("style", "clear:both")


                            externalexaminersdetails.Controls.Add(Div1)
                            externalexaminersdetails.Controls.Add(Div6)
                            externalexaminersdetails.Controls.Add(Div5)
                            externalexaminersdetails.Controls.Add(Div3)


                            Dim awardboardTextBox As New TextBox
                            Dim awardboardLabel As New Label
                            Dim awardboardDiv1 As New Div()

                            If withAwardBoard = True Then

                                awardboardTextBox.ValidationGroup = "updateexaminergroup"
                                awardboardTextBox.ID = "awardboard"
                                awardboardTextBox.Text = awardboard

                                awardboardLabel.Text = "Award board:"
                                awardboardLabel.AssociatedControlID = awardboardTextBox.ID

                                awardboardDiv1.Controls.Add(awardboardLabel)
                                awardboardDiv1.Controls.Add(awardboardTextBox)
                                awardboardDiv1.Attributes.Add("style", "clear:both")

                                updateButton = New Button
                                updateButton.ID = "updatebutton"
                                updateButton.Text = "Submit"
                                updateButton.CausesValidation = True
                                updateButton.ValidationGroup = "updateexaminergroup"
                                updateButton.Attributes.Add("onClientClick", "javascript:UpdateContent()")
                                AddHandler updateButton.Click, AddressOf updateData_Click

                                Dim editButtonDiv As New Div
                                editButtonDiv.CssClass = "sys_form-buttons"
                                editButtonDiv.Controls.Add(updateButton)

                                eeFieldset.Controls.Add(awardboardDiv1)
                                eeFieldset.Controls.Add(editButtonDiv)

                                ' awardUpdatePanel.Controls.Add(awardboardTextBox)
                                '  awardUpdatePanel.Controls.Add(editButtonDiv)
                                ' recordPanel.Controls.Add(awardUpdatePanel)
                                eeFieldset.Controls.Add(awardUpdateCompletePanel)
                            Else
                                If type = "External Examiner (Award)" Then
                                    awardboardTextBox.ValidationGroup = "updateexaminergroup"
                                    awardboardTextBox.ID = "awardboard"
                                    awardboardTextBox.Text = awardboard
                                    awardboardTextBox.Enabled = False

                                    awardboardLabel.Text = "Award board:"
                                    awardboardLabel.AssociatedControlID = awardboardTextBox.ID

                                    awardboardDiv1.Controls.Add(awardboardLabel)
                                    awardboardDiv1.Controls.Add(awardboardTextBox)
                                    awardboardDiv1.Attributes.Add("style", "clear:both")

                                    externalexaminersdetails.Controls.Add(awardboardDiv1)
                                End If
                            End If
                        End If
                    End While
                Else
                externalexaminersdetails.Controls.Add(New P("<strong>External examiner details not found. Please try the supplied link again or contact as.externalexaminers@solent.ac.uk</strong>"))
                End If

                Return externalexaminersdetails
            Catch ex As Exception
                response.Write("Error: createrecord():" + ex.ToString)
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try
        End Function

        'Protected Sub editLink_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        '    Me.FindControl("awardupdatepanel").Visible = True

        '    editLink = New LinkButton
        '    editLink.ID = "editlink"
        '    editLink.Text = "[Close]"
        '    AddHandler editLink.Click, AddressOf editLinkClose_Click

        'End Sub

        'Protected Sub editLinkClose_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        '    Me.FindControl("awardupdatepanel").Visible = False

        '    editLink = New LinkButton
        '    editLink.ID = "editlink"
        '    editLink.Text = "[Edit]"
        '    AddHandler editLink.Click, AddressOf editLink_Click
        'End Sub

        Protected Sub updateData_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Try
                Dim awardboardTextBox As TextBox = FindControl("awardboard")
                Dim awardboard As String = awardboardTextBox.Text


                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)

                myDatabaseUtil.addParameter("@examinerid", examinerid)
                myDatabaseUtil.addParameter("@awardboard", awardboard)

                myDatabaseUtil.executeNonQuery("storproc_updateexaminer")

                '  Me.FindControl("awardupdatepanel").Visible = False

                Me.FindControl("awardupdatecompletepanel").Controls.Add(New P("<strong>Award board successfully updated</strong>"))
                Me.FindControl("awardupdatecompletepanel").Visible = True


            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers updateData_Click: " & ex.Message())

            End Try

        End Sub

        Function checkExaminerStatus(ByVal examinerid As String)
            Dim status As String = ""

            Dim myDataReader As SqlDataReader = Nothing
            Try
                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                myDatabaseUtil.addParameter("@examinerid", examinerid)

                myDataReader = myDatabaseUtil.getSqlDataReader("storproc_getreportstatus")

                If myDataReader.HasRows Then
                    While myDataReader.Read()
                        If Not myDataReader("responsestatus") Is DBNull.Value Then
                            status = myDataReader("responsestatus")
                        End If
                    End While
                Else
                    status = "Not submitted"
                End If
                '   response.Write("status = " + status + "<br />")
            Catch ex As Exception
                HttpContext.Current.Response.Write("ERROR - ExternalExaminersListing_checkExaminerStatus: " & ex.Message())
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try

            Return status
        End Function


    End Class
End Namespace

