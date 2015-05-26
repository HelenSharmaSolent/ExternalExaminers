Imports System.Web
Imports CMS_API.WebUI.WebControls
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.ClientScriptManager
Imports System.Net.Mail
Imports edc.hs.portal2.ExternalExaminers

Namespace ExternalExaminers

    Public Class EmailAll

        Inherits BasePlaceHolderControl

        Dim sendExaminerEmailsButton As Button
        Dim selectPanel As New Panel
        Dim confirmPanel As New Panel
        Dim reportdate As String

        Private request As HttpRequest
        Private response As HttpResponse

        Public Sub addToOutput(ByVal controlIn As Object)
            Me.Controls.Add(controlIn)
        End Sub

        Private Sub ExternalExaminersEmailAll_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            request = HttpContext.Current.Request
            response = HttpContext.Current.Response

            Dim eeForm As New ExternalExaminers.Form
            reportdate = eeForm.getreportdate()

            selectPanel.ID = "selectpanel"
            addToOutput(selectPanel)
            Me.FindControl("selectpanel").Visible = True

            confirmPanel.ID = "confirmpanel"
            addToOutput(confirmPanel)
            Me.FindControl("confirmpanel").Visible = False


            Dim examinerTypeDropDown As New DropDownList
            examinerTypeDropDown.Items.Insert(0, New ListItem("Unit examiners", "unit"))
            examinerTypeDropDown.Items.Insert(0, New ListItem("Award examiners", "award"))
            examinerTypeDropDown.Items.Insert(0, New ListItem("All examiners", "all"))
            examinerTypeDropDown.ID = "examinerTypeDropDown"

            Dim examinerTypeLabel As New Label
            examinerTypeLabel.Text = "Select examiner type: "
            examinerTypeLabel.AssociatedControlID = examinerTypeDropDown.ID

            sendExaminerEmailsButton = New Button
            sendExaminerEmailsButton.ID = "updatebutton"
            sendExaminerEmailsButton.Text = "Send emails"
            sendExaminerEmailsButton.CausesValidation = True
            sendExaminerEmailsButton.ValidationGroup = "updateexaminergroup"
            sendExaminerEmailsButton.OnClientClick = "return confirm('Are you sure you want to send an email to the external examiners?');"
            AddHandler sendExaminerEmailsButton.Click, AddressOf sendExaminerEmails_Click

            Dim Div1 As New Div()
            Div1.Attributes.Add("style", "clear:both")
            Div1.Controls.Add(New P("Clicking on submit will send an automated email to all external examiners containing the link to their annual report."))
            Div1.Controls.Add(New P("<strong>Do not use without speaking to the quality officer in Academic Services.</strong>"))
            Div1.Controls.Add(examinerTypeLabel)
            Div1.Controls.Add(examinerTypeDropDown)


            Dim Div2 As New Div()
            Div2.Controls.Add(sendExaminerEmailsButton)

            selectPanel.Controls.Add(Div1)
            selectPanel.Controls.Add(Div2)

        End Sub



        Protected Sub sendExaminerEmails_Click(ByVal sender As Object, ByVal e As System.EventArgs)
            Dim examinerTypeDropDown As DropDownList = FindControl("examinerTypeDropDown")
            Dim examinerType As String = examinerTypeDropDown.Text
            Dim myDataReader As SqlDataReader = Nothing
            Try
                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)

                myDatabaseUtil.addParameter("@type", examinerType)

                myDataReader = myDatabaseUtil.getSqlDataReader("storproc_emailexaminers")


                If myDataReader.HasRows Then


                    Dim id As String = ""
                    Dim title As String = ""
                    Dim firstname As String = ""
                    Dim lastname As String = ""
                    Dim name As String = ""
                    Dim email As String = ""
                    Dim type As String = ""

                    Dim completedDiv As New Div
                    completedDiv.Controls.Add(New P("Emails have been sent to the following external examiners:"))


                    While myDataReader.Read()


                        If Not myDataReader("examinerid") Is DBNull.Value Then
                            id = myDataReader("examinerid")
                        End If

                        If Not myDataReader("title") Is DBNull.Value Then
                            title = myDataReader("title")
                        End If

                        If Not myDataReader("first_name") Is DBNull.Value Then
                            firstname = myDataReader("first_name")
                        End If

                        If Not myDataReader("last_name") Is DBNull.Value Then
                            lastname = myDataReader("last_name")
                        End If

                        If Not myDataReader("email") Is DBNull.Value Then
                            email = myDataReader("email")
                        End If

                        If Not myDataReader("notes") Is DBNull.Value Then
                            type = myDataReader("notes")
                        End If

                        name = title + " " + firstname + " " + lastname


                        sendEmail(type, id, name, email)
                        completedDiv.Controls.Add(New P(name + " - " + email + " - " + type))


                    End While


                    confirmPanel.Controls.Add(completedDiv)

                    Me.FindControl("selectpanel").Visible = False
                    Me.FindControl("confirmpanel").Visible = True

                Else
                    Dim completedDiv As New P("No examiners of the selected type where found.")
                    confirmPanel.Controls.Add(completedDiv)

                    Me.FindControl("selectpanel").Visible = False
                    Me.FindControl("confirmpanel").Visible = True

                End If

            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers sendEmails: " & ex.Message())

            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try


        End Sub

        Public Sub sendEmail(ByVal type As String, ByVal id As String, ByVal name As String, ByVal email As String)

            Try
                Dim client As New SmtpClient("server-mail.solent.ac.uk")
                Dim toAddress As New MailAddress("rosemary.bock@solent.ac.uk")
                Dim fromAddress As New MailAddress("asqs.externalexaminers@solent.ac.uk", "Southampton Solent University")
                Dim message As New MailMessage(fromAddress, toAddress)
                message.CC.Add("helen.sharma@solent.ac.uk")
                Dim formurl As String = ""
                Dim body As String = ""
                Dim sentence1 As String = ""
                Dim typesubjecttext As String = ""

                ' response.Write("reportdate = " + reportdate)

                If type = "External Examiner (Unit)" Then
                    type = "unit"
                    sentence1 = "completing your moderation of student work in the summer, or if you have been invited to attend, your main assessment board. "
                    formurl = "http://portal.solent.ac.uk/portal-apps/external-examiners/feedback/form.aspx?id=" + id + "&type=" + type
                    typesubjecttext = "Unit "
                ElseIf type = "External Examiner (Award)" Then
                    type = "award"
                    sentence1 = "your main Progression and Award Board. "
                    formurl = "http://portal.solent.ac.uk/portal-apps/external-examiners/feedback/form.aspx?id=" + id + "&type=" + type
                    typesubjecttext = "Progression and award "
                End If

                message.IsBodyHtml = True
                message.Subject = typesubjecttext + "external examiners annual report " + reportdate

                body = body + "<p><strong>" + typesubjecttext + "external examiners annual report " + reportdate + "</strong></p>" + vbCrLf
                body = body + "<p>I write to inform you of the arrangements for submitting your annual report.</p>" + vbCrLf
                body = body + "<p>External examiner reports should be submitted within four weeks of " + sentence1 + "Your report must be completed before the external examiner fee can be claimed.</p>" + vbCrLf
                body = body + "<p>Follow these instructions to access your personalised annual report pro-forma. This should be filled in and submitted online. </p>" + vbCrLf
                body = body + "<ol><li>Click on this link: "
                body = body + "<a href=""" + formurl + """>" + formurl + "</a></li><li>You will redirected to the login screen for the University's staff and student portal</li>" + vbCrLf
                body = body + "<li>Enter the following login details (these login details apply only to completing your annual report template):<ul><li>Username: externalexaminer</li><li>Password: f33dback865</li></ul></li>"
                body = body + "<li>You will be redirected to your annual report.</li></ol>"
                body = body + "<p>Guidance on completing the annual report can be found on the University's external examiner website <a href=""http://www.solent.ac.uk/externalexaminers"">www.solent.ac.uk/externalexaminers</a> along with frequently asked questions. </p>" + vbCrLf
                body = body + "<p>Your report will be widely read and will be considered by Academic Services. It will also be considered formally by the course team(s) and the faculty management team as part of the course review process. A summary of themes emerging from external examiner reports is presented to the University’s Academic Board. Information contained in external examiner reports will constitute recorded information held by the University and will therefore be open to disclosure, if requested by any person under the Freedom of Information Act. </p>" + vbCrLf
                body = body + "<p><strong>Please note: Individual students or members of staff should not be identified in the report.</strong><br /><br />The University values the input of external examiners and is grateful for the work you undertake. We look forward to receiving your report and thank you for your interest in the University and our students.</p>" + vbCrLf
                body = body + "<p>If you have any questions relating to your annual report or University level matters, please do contact me. If your query is faculty level, please contact the appropriate quality office.</p>" + vbCrLf
                body = body + "<p>Kind regards</p>" + vbCrLf
                body = body + "<p>Andrew Stevenson<br />Quality Officer, Academic Services" + vbCrLf
                body = body + "<br />Southampton Solent University<br /> " + vbCrLf
                body = body + "<br />Tel: +44 (0)23 8201 3978" + vbCrLf
                body = body + "<br />Email: <a href=""mailto:as.externalexaminers@solent.ac.uk"">as.externalexaminers@solent.ac.uk</a></p>" + vbCrLf

                message.Body = body
                client.Send(message)

            Catch ex As Exception
                response.Write("ERROR - External Examiner sendEmail: " + ex.Message + " - " + ex.InnerException.ToString)
            End Try
        End Sub
    End Class
End Namespace

