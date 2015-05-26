Imports System.Web
Imports CMS_API.WebUI.WebControls
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.ClientScriptManager


Namespace ExternalExaminers
    Public Class DataRefresh

        Inherits BasePlaceHolderControl

        Dim dataRefreshButton As Button
        Dim formPanel As New Panel
        Dim confirmPanel As New Panel

        Private request As HttpRequest
        Private response As HttpResponse

        Public Sub addToOutput(ByVal controlIn As Object)
            Me.Controls.Add(controlIn)
        End Sub

        Private Sub ExternalExaminersDataRefresh_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init


            Try
                request = HttpContext.Current.Request
                response = HttpContext.Current.Response

                formPanel.ID = "formpanel"
                addToOutput(formPanel)
                Me.FindControl("formpanel").Visible = True

                confirmPanel.ID = "confirmpanel"
                addToOutput(confirmPanel)
                Me.FindControl("confirmpanel").Visible = False

                dataRefreshButton = New Button
                dataRefreshButton.ID = "datarefreshbutton"
                dataRefreshButton.Text = "Refresh external examiner data"
                dataRefreshButton.CausesValidation = False
                AddHandler dataRefreshButton.Click, AddressOf dataRefresh_Click

                formPanel.Controls.Add(New P("Pressing this button will push the latest external examiners data from the Quercus student record system into the external examiners feedback system. "))

                formPanel.Controls.Add(dataRefreshButton)

            Catch ex As Exception
                response.Write("ERROR - ExternalExaminersDataRefresh_Init: " & ex.Message())
            End Try

        End Sub

        Protected Sub dataRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Try
                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)

                myDatabaseUtil.addOutputParameter("@RecCount")
                myDatabaseUtil.executeNonQuery("storproc_bas_datarefreshfromquercus")

                Dim reccount As SqlParameter = myDatabaseUtil.getParameter("@RecCount")

                Dim recordcount As String = ""
                recordcount = reccount.Value.ToString()

                If recordcount > 0 Then
                    confirmPanel.Controls.Add(New P("The external examiner data has now been refreshed"))
                    confirmPanel.Controls.Add(New P("Total number of records processed: " + recordcount))
                Else
                    confirmPanel.Controls.Add(New P("There has been a problem refreshing the data."))
                End If

            Catch ex As Exception
                HttpContext.Current.Response.Write("ERROR - refreshData_Click: " & ex.Message())
            End Try


            formPanel.Visible = False
            confirmPanel.Visible = True


        End Sub
    End Class
End Namespace

