Imports System.Web
Imports CMS_API.WebUI.WebControls
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.ClientScriptManager
Imports edc.hs.portal2.ExternalExaminers
Imports Microsoft.Office
Imports Microsoft.Office.Interop


Namespace ExternalExaminers

    Public Class Listing

        Inherits BasePlaceHolderControl

        Dim WithEvents filterLinkButton As Button
        Dim editLinkButton As Button
        Dim wordLinkButton As LinkButton
        Dim PortalUser
        Dim eeRecord As Record
        Dim reportyear As String

        Private request As HttpRequest
        Private response As HttpResponse

        Public Sub addToOutput(ByVal controlIn As Object)
            Me.Controls.Add(controlIn)
        End Sub

        Private Sub JobRequestListing_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            PortalUser = DirectCast(Me.Page.FindControl("PortalHelper"), ljm.portal2.Security.PortalHelper).getPortalUser()

            createlisting("all", "all")
        End Sub

        Sub createlisting(ByVal examinerTypeIn As String, ByVal statusIn As String)

            Dim eeform As New ExternalExaminers.Form
            reportyear = eeform.getreportdate()

            request = HttpContext.Current.Request
            response = HttpContext.Current.Response


            If PortalUser.getGroup = "public" Or PortalUser.getGroup = "student" Then
                addToOutput(New Span("Only authorised members of staff can access this application"))

            Else

                Dim outputDiv As New Div
                outputDiv.ID = "external-examiners-feedback-application"
                addToOutput(outputDiv)


                Dim filterDiv As New Div
                filterDiv.CssClass = "filter-box"

                Dim examinerTypeFilterDropDown As New DropDownList()
                Dim statusFilterDropDown As New DropDownList()

             

                Try

                    examinerTypeFilterDropDown.ID = "examinerTypeFilterDropDown"
                    examinerTypeFilterDropDown.Items.Add(New ListItem("View all", "all"))
                    examinerTypeFilterDropDown.Items.Add(New ListItem("Unit Examiner", "unit"))
                    examinerTypeFilterDropDown.Items.Add(New ListItem("Award Examiner", "award"))

                    Dim examinerTypeDropDownLabel As New Label
                    examinerTypeDropDownLabel.Text = "Examiner type: "
                    examinerTypeDropDownLabel.AssociatedControlID = examinerTypeFilterDropDown.ID

                    Dim examinerTypeDropDownDiv As New Div()
                    examinerTypeDropDownDiv.Controls.Add(examinerTypeDropDownLabel)
                    examinerTypeDropDownDiv.Controls.Add(examinerTypeFilterDropDown)
                    examinerTypeDropDownDiv.Attributes.Add("style", "display:inline")

                    statusFilterDropDown.ID = "statusFilterdropdown"
                    statusFilterDropDown.Items.Add(New ListItem("View all", "all"))
                    statusFilterDropDown.Items.Add(New ListItem("Submitted", "completed"))
                    statusFilterDropDown.Items.Add(New ListItem("In progress", "saved"))
                    statusFilterDropDown.Items.Add(New ListItem("Not submitted", "uncompleted"))

                    Dim statusDropDownLabel As New Label
                    statusDropDownLabel.Text = "Status: "
                    statusDropDownLabel.AssociatedControlID = statusFilterDropDown.ID

                    Dim statusDropDownDiv As New Div()
                    statusDropDownDiv.Controls.Add(statusDropDownLabel)
                    statusDropDownDiv.Controls.Add(statusFilterDropDown)
                    statusDropDownDiv.Attributes.Add("style", "display:inline")

                    filterLinkButton = New Button
                    filterLinkButton.Text = "GO"
                    filterLinkButton.ID = "filterlinkbutton"
                    filterLinkButton.CssClass = "small-go-button"
                    filterLinkButton.CausesValidation = False
                    AddHandler filterLinkButton.Click, AddressOf filterItem_Click

                    filterDiv.Controls.Add(New Span("<span><strong>Filter:&nbsp;</strong></span>"))
                    filterDiv.Controls.Add(examinerTypeDropDownDiv)
                    filterDiv.Controls.Add(statusDropDownDiv)
                    filterDiv.Controls.Add(filterLinkButton)


                    outputDiv.Controls.Add(filterDiv)

                Catch ex As Exception
                    HttpContext.Current.Response.Write("ERROR - ExternalExaminersListing_CreateFilter: " & ex.Message())

                End Try

                Dim myDataReader As SqlDataReader = Nothing
                Try

                    Dim myPortalUser = DirectCast(Me.Page.FindControl("PortalHelper"), ljm.portal2.Security.PortalHelper).getPortalUser()

                    Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)

                    myDatabaseUtil.addParameter("@type", examinerTypeIn)
                    myDatabaseUtil.addParameter("@status", statusIn)
                    myDatabaseUtil.addParameter("@reportyear", reportyear)

                    myDataReader = myDatabaseUtil.getSqlDataReader("storproc_listexaminers")

                    Dim iRow As Integer = 0
                    Dim rowColour As String = "#fff"

                    Dim resultsText As New P("")
                    outputDiv.Controls.Add(resultsText)

                    If myDataReader.HasRows Then

                        ' Dim listingDiv As New Div()

                        Dim myTable As New Table
                        Dim headerRow As New TableHeaderRow
                        headerRow.Attributes.Add("class", "headerow")
                        headerRow.Attributes.Add("style", "font-weight:bold; color: #fff")

                        Dim examinerLinkHeaderCell As New TableHeaderCell
                        examinerLinkHeaderCell.Text = "Name"

                        Dim descHeaderCell As New TableHeaderCell
                        descHeaderCell.Text = "Type"

                        Dim statusHeaderCell As New TableHeaderCell
                        statusHeaderCell.Text = "Status"

                        Dim reportLinkHeaderCell As New TableHeaderCell
                        reportLinkHeaderCell.Text = "Report"

                        Dim wordVersionHeaderCell As New TableHeaderCell
                        wordVersionHeaderCell.Text = "Export"

                        headerRow.Controls.Add(examinerLinkHeaderCell)
                        headerRow.Controls.Add(descHeaderCell)
                        headerRow.Controls.Add(statusHeaderCell)
                        headerRow.Controls.Add(reportLinkHeaderCell)
                        headerRow.Controls.Add(wordVersionHeaderCell)

                        myTable.Controls.Add(headerRow)

                        While myDataReader.Read()

                            If iRow Mod 2 = 0 Then
                                rowColour = "#fff"
                            Else
                                rowColour = "#f9f9f9"
                            End If

                            Dim id As String = myDataReader("examinerid")
                            Dim name As String = myDataReader("title") + " " + myDataReader("first_name") + " " + myDataReader("last_name")
                            Dim type As String = myDataReader("notes")


                            Dim tableRow As New TableRow
                            tableRow.Attributes.Add("style", "color: #000; background-color: " + rowColour)

                            Dim examinerLinkTableCell As New TableCell
                            examinerLinkTableCell.Controls.Add(New Span("<a href=""/portal-apps/external-examiners/details.aspx?id=" + id + """>" + name + "</a>"))

                            Dim descTableCell As New TableCell
                            descTableCell.Controls.Add(New Span(type))

                          

                            eeRecord = New Record
                            Dim statusTableCell As New TableCell

                            Dim status As String = eeRecord.checkExaminerStatus(id)

                            If status = "submitted" Then
                                status = "Report submitted"
                            End If

                            If status = "saved" Then
                                status = "In progress"
                            End If

                            statusTableCell.Controls.Add(New Span(status))
                            Dim reportLinkTableCell As New TableCell
                            Dim reportLink = ""
                            If type = "External Examiner (Unit)" Then
                                reportLink = "?id=" + id + "&type=unit&adminmode=true"">Go to unit report</a>"
                            ElseIf type = "External Examiner (Award)" Then
                                reportLink = "?id=" + id + "&type=award&adminmode=true"">Go to award report</a>"
                            End If
                            reportLink = "<a href=""/portal-apps/external-examiners/feedback/form.aspx" + reportLink

                            reportLinkTableCell.Controls.Add(New Span(reportLink))

                            Dim wordVersionTableCell As New TableCell

                            Dim wordreportLink = ""
                            Dim wordreporturl As String = ""

                            If type = "External Examiner (Unit)" Then
                                wordreporturl = "?id=" + id + "&type=unit"">Word</a>"
                            ElseIf type = "External Examiner (Award)" Then
                                wordreporturl = "?id=" + id + "&type=award"">Word</a>"
                            End If

                            wordreportLink = "<a class=""word-download-link-small"" href=""/portal-apps/external-examiners/word-version.aspx" + wordreporturl


                            Dim pdfreporturl As String = ""
                            Dim pdfreportLink = ""

                            If type = "External Examiner (Unit)" Then
                                pdfreporturl = "?id=" + id + "&type=unit&print=true"">PDF</a>"
                            ElseIf type = "External Examiner (Award)" Then
                                pdfreporturl = "?id=" + id + "&type=award&print=true"">PDF</a>"
                            End If

                            pdfreportLink = "<a class=""pdf-download-link-small"" href=""/portal-apps/external-examiners/feedback/form.aspx" + pdfreporturl


                            wordVersionTableCell.Controls.Add(New Span(wordreportLink + " " + pdfreportLink))

                            tableRow.Controls.Add(examinerLinkTableCell)
                            tableRow.Controls.Add(descTableCell)
                            tableRow.Controls.Add(statusTableCell)
                            tableRow.Controls.Add(reportLinkTableCell)
                            tableRow.Controls.Add(wordVersionTableCell)

                            myTable.Controls.Add(tableRow)

                            iRow = iRow + 1



                        End While

                        outputDiv.Controls.Add(myTable)

                    Else

                        outputDiv.Controls.Add(New P("There are no external examiners that match your search criteria."))
                    End If
                    myDataReader.NextResult()

                    If myDataReader.HasRows Then

                        While myDataReader.Read()
                            resultsText.Controls.Add(New P("Search returned " + myDataReader("myrowcounter").ToString + " results"))
                        End While
                    End If




                Catch ex As Exception
                    HttpContext.Current.Response.Write("ERROR - createlisting() " & ex.Message())
                Finally
                    ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
                End Try



            End If

        End Sub

        Protected Sub filterItem_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim examinerTypeDropDown As DropDownList = FindControl("examinerTypeFilterDropDown")
            Dim examinerType As String = examinerTypeDropDown.Text

            Dim statusDropDown As DropDownList = FindControl("statusFilterDropDown")
            Dim status As String = statusDropDown.Text

            statusDropDown.SelectedItem.Value = status
            examinerTypeDropDown.SelectedItem.Value = examinerType

            Try

                Me.Controls.Clear()
                createlisting(examinerType, status)

            Catch ex As Exception
                HttpContext.Current.Response.Write("ERROR - filterItem_Click: " & ex.Message())

            End Try
        End Sub


    End Class

End Namespace
