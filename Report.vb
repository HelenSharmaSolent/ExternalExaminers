Imports System.Web
Imports CMS_API.WebUI.WebControls
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.ClientScriptManager
Imports System.Web.UI
Imports edc.hs.portal2.ExternalExaminers.Record
Imports edc.ljm.common

Namespace ExternalExaminers

    Public Class Report

        Inherits BasePlaceHolderControl

        Dim reportsubmitButton As Button
        Dim selectPanel As New Panel
        Dim confirmPanel As New Panel
        Dim datasource As New SqlDataSource
        Dim eeFieldset As New FieldSet


        Private request As HttpRequest
        Private response As HttpResponse

        Public Sub addToOutput(ByVal controlIn As Object)
            Me.Controls.Add(controlIn)
        End Sub

        Private Sub ExternalExaminersReport_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            request = HttpContext.Current.Request
            response = HttpContext.Current.Response

            eeFieldset.ID = "reportfieldset"
            addToOutput(eeFieldset)

            selectPanel.ID = "selectpanel"
            eeFieldset.Controls.Add(selectPanel)
            Me.FindControl("selectpanel").Visible = True

            confirmPanel.ID = "confirmpanel"
            eeFieldset.Controls.Add(confirmPanel)
            Me.FindControl("confirmpanel").Visible = True

            Dim examinerTypeDropDown As New DropDownList
            examinerTypeDropDown.Items.Insert(0, New ListItem("Unit examiners - individual unit responses", "unitresponses"))
            examinerTypeDropDown.Items.Insert(0, New ListItem("Unit examiners report", "unit"))
            examinerTypeDropDown.Items.Insert(0, New ListItem("Award and progression examiners report", "award"))
            examinerTypeDropDown.ID = "examinerTypeDropDown"

            Dim examinerTypeLabel As New Label
            examinerTypeLabel.Text = "Select report type: "
            examinerTypeLabel.AssociatedControlID = examinerTypeDropDown.ID

            Dim Div1 As New Div()
            Div1.Attributes.Add("style", "clear:both")
            Div1.Controls.Add(examinerTypeLabel)
            Div1.Controls.Add(examinerTypeDropDown)

            Dim yearDropDown As New DropDownList
            yearDropDown.Items.Insert(0, New ListItem("2010/11", "2011"))
            yearDropDown.Items.Insert(0, New ListItem("2011/12", "2012"))
            yearDropDown.Items.Insert(0, New ListItem("2012/13", "2013"))
            yearDropDown.Items.Insert(0, New ListItem("2013/14", "2014"))
            yearDropDown.Items.Insert(0, New ListItem("2014/15", "2015"))
            yearDropDown.Items.Insert(0, New ListItem("2015/16", "2016"))
            yearDropDown.Items.Insert(0, New ListItem("2016/17", "2017"))
            yearDropDown.ID = "yearDropDown"

            Dim yearLabel As New Label
            yearLabel.Text = "Select year: "
            yearLabel.AssociatedControlID = yearDropDown.ID

            Dim Div2 As New Div()
            Div2.Attributes.Add("style", "clear:both")
            Div2.Controls.Add(yearLabel)
            Div2.Controls.Add(yearDropDown)

            reportsubmitButton = New Button
            reportsubmitButton.ID = "reportsubmitbutton"
            reportsubmitButton.Text = "Download Excel report"
            AddHandler reportsubmitButton.Click, AddressOf submit_Click

            Dim Div3 As New Div()
            Div3.Controls.Add(reportsubmitButton)

            selectPanel.Controls.Add(Div1)
            selectPanel.Controls.Add(Div2)
            selectPanel.Controls.Add(Div3)

            datasource.ID = "excelreport"
            confirmPanel.Controls.Add(datasource)

        End Sub



        Protected Sub submit_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Dim examinerTypeDropDown As DropDownList = FindControl("examinerTypeDropDown")
            Dim type As String = examinerTypeDropDown.Text

            Dim yearDropDown As DropDownList = FindControl("yearDropDown")
            Dim year As String = yearDropDown.Text
            Dim storproc As String = ""

            Dim myDatabaseUtil As New DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
            Dim mySqlDataReader As SqlDataReader = Nothing

            Dim yearrange, year1, year2
            year2 = Right(year, 2)
            year1 = CInt(year) - 1
            yearrange = year1.ToString + "/" + year2


            If type = "unitresponses" Then
                storproc = "storproc_excelreportunitresponses"
                year = yearrange
            Else
                If year < 2015 Then
                    storproc = "storproc_excelreportpre2015"
                Else '2015 and over use different database tables

                    storproc = "storproc_excelreportpost2015"
                    year = yearrange
                    myDatabaseUtil.addParameter("responsestatus", "submitted")
                End If
            End If

            response.Write("stor proc " + storproc + "<br />")
            response.Write("year " + year.ToString + "<br />")
            response.Write("type " + type.ToString + "<br />")

            Try

                myDatabaseUtil.addParameter("year", year.ToString)
                myDatabaseUtil.addParameter("type", type.ToString)

                mySqlDataReader = myDatabaseUtil.getSqlDataReader(storproc)
            Catch ex As Exception
                response.Write("ERROR - Report submit_click: " & ex.Message())
            End Try

            Dim myExcelReport As New edc.ljm.common.Export.ExportAsXLS(type + "-examiner-report-" + year, mySqlDataReader)
            Me.addToOutput(myExcelReport)

        End Sub

        Public Shared Sub Export(fileName As String, gv As GridView)
            HttpContext.Current.Response.Clear()
            HttpContext.Current.Response.AddHeader("content-disposition", String.Format("attachment; filename={0}", fileName))
            HttpContext.Current.Response.ContentType = "application/ms-excel"
            HttpContext.Current.Response.Write("<head><meta http-equiv=Content-Type content=:" & """"c & "text/html; charset=utf-8" & """"c & "></head>")
            Using sw As New System.IO.StringWriter()
                Using htw As New System.Web.UI.HtmlTextWriter(sw)
                    Dim table As New Table()
                    If gv.HeaderRow IsNot Nothing Then
                        PrepareControlForExport(gv.HeaderRow)
                        table.Rows.Add(gv.HeaderRow)
                    End If
                    For Each row As GridViewRow In gv.Rows
                        PrepareControlForExport(row)
                        table.Rows.Add(row)
                    Next
                    If gv.FooterRow IsNot Nothing Then
                        PrepareControlForExport(gv.FooterRow)
                        table.Rows.Add(gv.FooterRow)
                    End If
                    table.GridLines = CType(3, GridLines)
                    table.RenderControl(htw)
                    HttpContext.Current.Response.Write(sw.ToString())
                    HttpContext.Current.Response.[End]()
                End Using
            End Using
        End Sub

        Private Shared Sub PrepareControlForExport(control As Control)
            For i As Integer = 0 To control.Controls.Count - 1
                Dim current As Control = control.Controls(i)
                If TypeOf current Is LinkButton Then
                    control.Controls.Remove(current)
                    control.Controls.AddAt(i, New LiteralControl(TryCast(current, LinkButton).Text))
                ElseIf TypeOf current Is ImageButton Then
                    control.Controls.Remove(current)
                    control.Controls.AddAt(i, New LiteralControl(TryCast(current, ImageButton).AlternateText))
                ElseIf TypeOf current Is HyperLink Then
                    control.Controls.Remove(current)
                    control.Controls.AddAt(i, New LiteralControl(TryCast(current, HyperLink).Text))
                ElseIf TypeOf current Is System.Web.UI.WebControls.DropDownList Then
                    control.Controls.Remove(current)
                    control.Controls.AddAt(i, New LiteralControl(TryCast(current, System.Web.UI.WebControls.DropDownList).SelectedItem.Text))
                ElseIf TypeOf current Is CheckBox Then
                    control.Controls.Remove(current)
                    control.Controls.AddAt(i, New LiteralControl(If(TryCast(current, CheckBox).Checked, "True", "False")))
                    'fudge for phone numbers - specific for this table
                ElseIf TypeOf current Is DataControlFieldCell Then
                    Dim myControl As DataControlFieldCell = TryCast(current, DataControlFieldCell)
                    If myControl.Text IsNot Nothing AndAlso myControl.Text.Length > 4 Then
                        If IsNumeric(myControl.Text) Then
                            current.Controls.Add(New LiteralControl(myControl.Text.Insert(5, " ")))
                        End If

                        If myControl.Text = "Please select..." Then
                            current.Controls.Add(New LiteralControl(" "))
                        End If
                    End If
                End If

                If current.HasControls() Then
                    PrepareControlForExport(current)
                End If
            Next
        End Sub

    End Class
End Namespace

