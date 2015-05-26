Imports System.Web
Imports CMS_API.WebUI.WebControls
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.ClientScriptManager
Imports System.Web.SessionState
Imports edc.hs.portal2.ExternalExaminers.Record
Imports System.Net.Mail


Namespace ExternalExaminers
    Public Class Form

        Inherits BasePlaceHolderControl

        Const numberofunitpanels As Int32 = 7
        Const numberofawardpanels As Int32 = 4

        Dim reportyear As String = ""
        Dim formPanel0 As New Panel
        Dim formPanel1 As New Panel
        Dim formPanel2 As New Panel
        Dim formPanel3 As New Panel
        Dim formPanel4 As New Panel
        Dim formPanel5 As New Panel
        Dim formPanel6 As New Panel
        Dim formPanel7 As New Panel
        Dim confirmPanel As New Panel
        Dim savenotificationPanel As New Panel
        Dim savenotificationPanel2 As New Panel

        Dim WithEvents prevButton As Button
        Dim WithEvents nextButton As Button
        Dim WithEvents saveButton As Button
        Dim WithEvents saveButton2 As Button
        Dim WithEvents submitButton As Button
        Dim WithEvents exportPDFButton As Button
        Dim WithEvents toggleViewButton As LinkButton
        Dim panelnavigation As New Div
        Dim saveExplanation As New Div
        Dim eeFieldset As New FieldSet
        Dim externalexaminersdetails As New Div
        Dim updateButtonDiv As New Div
        Dim currentpanel As New Int32
        Dim eeRecord As New Record
        Dim status As String
        '   Dim isSaved As Boolean = False
        '  Dim isSubmitted As Boolean = False
        Dim examinerid As String
        Dim examinerType As String
        Dim currenttoggleview As String
        Dim questionid As String = ""
        Dim isExistsYesRadioButton As Boolean = False
        Dim isAdminMode As Boolean = False

        Private request As HttpRequest
        Private response As HttpResponse
        Private session As HttpSessionState

        Public Sub addToOutput(ByVal controlIn As Object)
            Me.Controls.Add(controlIn)
        End Sub

        Public Sub New()
            request = HttpContext.Current.Request
            response = HttpContext.Current.Response
            examinerid = request.QueryString("id")
            examinerType = request.QueryString("type")
        End Sub

        Private Sub ExternalExaminersFormUnit_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

            session = HttpContext.Current.Session
            request = HttpContext.Current.Request
            response = HttpContext.Current.Response


            If String.IsNullOrEmpty(request.QueryString("id")) Or String.IsNullOrEmpty(request.QueryString("type")) Then

                addToOutput(New P("Examiner ID not supplied. Please try the supplied link again or contact as.externalexaminers@solent.ac.uk"))

            Else 'run application
                ' response.Write("hello??????")

                ' response.Write("timeout is = " + 

                Try

                    reportyear = getreportdate()

                    examinerid = request.QueryString("id")
                    examinerType = request.QueryString("type")
                    status = eeRecord.checkExaminerStatus(examinerid)

                    If Not String.IsNullOrEmpty(request.QueryString("adminmode")) Then
                        isAdminMode = True
                    End If


                Catch ex As Exception
                    response.Write("ERROR: ExternalExaminersFormUnit_Init 1 " + ex.ToString)
                End Try


                Try
                    If Page.IsPostBack Then
                        ' response.Write("postback<br />")
                        currentpanel = session("currentpanel")
                    Else
                        ' response.Write("not postback<br />")
                        session("currentpanel") = 0
                        currentpanel = 0
                    End If


                    'create buttons
                    prevButton = New Button
                    prevButton.ID = "prevbutton"
                    prevButton.Text = "Previous"
                    prevButton.CssClass = "button-previous"
                    prevButton.CausesValidation = False
                    prevButton.ValidationGroup = "externalexaminersvalidationgroup"
                    prevButton.Visible = False
                    'AddHandler prevButton.Click, AddressOf prevButton_Click

                    nextButton = New Button
                    nextButton.ID = "nextbutton"
                    nextButton.Text = "Next"
                    nextButton.CssClass = "button-next"
                    nextButton.CausesValidation = False
                    nextButton.ValidationGroup = "externalexaminersvalidationgroup"
                    'AddHandler nextButton.Click, AddressOf nextButton_Click

                    exportPDFButton = New Button
                    exportPDFButton.ID = "savebutton"
                    exportPDFButton.Text = "Save"
                    exportPDFButton.CssClass = "button-next"
                    exportPDFButton.CausesValidation = False
                    exportPDFButton.ValidationGroup = "externalexaminersvalidationgroup"
                    AddHandler exportPDFButton.Click, AddressOf exportPDFButton_Click

                    saveButton = New Button
                    saveButton.ID = "savebutton"
                    saveButton.Text = "Save draft"
                    saveButton.CssClass = "button-save image-align-right"
                    saveButton.CausesValidation = False
                    saveButton.ValidationGroup = "externalexaminersvalidationgroup"
                    AddHandler saveButton.Click, AddressOf saveButton_Click

                    saveButton2 = New Button
                    saveButton2.ID = "savebutton2"
                    saveButton2.Text = "Save draft"
                    saveButton2.CssClass = "button-save"
                    saveButton2.CausesValidation = False
                    saveButton2.ValidationGroup = "externalexaminersvalidationgroup"
                    AddHandler saveButton2.Click, AddressOf saveButton_Click

                    submitButton = New Button
                    submitButton.ID = "submitbutton"
                    submitButton.Text = "Submit"
                    submitButton.CssClass = "button-next"
                    submitButton.CausesValidation = True
                    submitButton.ValidationGroup = "externalexaminersvalidationgroup"
                    AddHandler submitButton.Click, AddressOf submitButton_Click


                    'create container div and add to output
                    Dim outputDiv As New Div
                    outputDiv.ID = "external-examiners-feedback-application"
                    addToOutput(outputDiv)

                    'create toggle button and add to container 
                    toggleViewButton = New LinkButton
                    toggleViewButton.ID = "toggleviewbutton"
                    toggleViewButton.Text = "Toggle form view"
                    toggleViewButton.CssClass = "toggle-link image-align-right"
                    toggleViewButton.CausesValidation = False
                    toggleViewButton.ValidationGroup = "externalexaminersvalidationgroup"
                    AddHandler toggleViewButton.Click, AddressOf toggleViewButton_Click

                    Dim text1 As New P
                    text1.Attributes.Add("style", "clear:both")
                    text1.InnerHTML = "<strong>Your report has been saved</strong>"

                    savenotificationPanel.ID = "externalexaminerssavenotificationpanel"
                    savenotificationPanel.CssClass = "savenotification"
                    savenotificationPanel.Controls.Add(text1)
                    savenotificationPanel.Visible = False

                    savenotificationPanel2.ID = "externalexaminerssavenotificationpanel2"
                    savenotificationPanel2.CssClass = "savenotification"
                    savenotificationPanel2.Controls.Add(New P("<strong>Your report has been saved</strong>"))
                    savenotificationPanel2.Visible = False

                    Dim savetext As New Div
                    savetext.CssClass = "savetext"
                    savetext.Controls.Add(New P("Use the 'save draft' button to save your form at any time but you must <strong>submit your final responses using the 'submit' button</strong> at the end of the form once complete."))
                    savetext.Controls.Add(New P("We advise you to save your form regularly to prevent any loss of entered data."))


                    Dim saveButtonDiv As New Div
                    saveButtonDiv.CssClass = "savebuttoncontainer"
                    saveButtonDiv.Controls.Add(saveButton)
                    saveButtonDiv.Controls.Add(savenotificationPanel)

                    saveExplanation.CssClass = "savexplanation"
                    saveExplanation.Controls.Add(savetext)
                    saveExplanation.Controls.Add(saveButtonDiv)
                    outputDiv.Controls.Add(saveExplanation)

                    panelnavigation.ID = "panelnavigation"
                    outputDiv.Controls.Add(panelnavigation)

                    Dim panelnavigationlist As New UL
                    panelnavigation.Controls.Add(panelnavigationlist)
                    panelnavigation.Controls.Add(toggleViewButton)

                    'set up panels
                    formPanel0.ID = "externalexaminersformpanel0"
                    formPanel0.Attributes.Add("class", "panel")
                    formPanel1.ID = "externalexaminersformpanel1"
                    formPanel1.Attributes.Add("class", "panel")
                    formPanel2.ID = "externalexaminersformpanel2"
                    formPanel2.Attributes.Add("class", "panel")
                    formPanel3.ID = "externalexaminersformpanel3"
                    formPanel3.Attributes.Add("class", "panel")
                    formPanel4.ID = "externalexaminersformpanel4"
                    formPanel4.Attributes.Add("class", "panel")
                    If examinerType = "unit" Then
                        formPanel5.ID = "externalexaminersformpanel5"
                        formPanel5.Attributes.Add("class", "panel")
                        formPanel6.ID = "externalexaminersformpanel6"
                        formPanel6.Attributes.Add("class", "panel")
                        formPanel7.ID = "externalexaminersformpanel7"
                        formPanel7.Attributes.Add("class", "panel")
                    End If
                    confirmPanel.ID = "externalexaminersconfirmpanel"

                    eeFieldset.ID = "externalexaminersfieldset"


                    Dim myValidationSummary As New System.Web.UI.WebControls.ValidationSummary
                    myValidationSummary.ID = "externalexaminersvalidationsummary"
                    myValidationSummary.ValidationGroup = "externalexaminersvalidationgroup"
                    myValidationSummary.HeaderText = "Please correct the following issues before submitting:"
                    eeFieldset.Controls.Add(myValidationSummary)

                    'add panels to fieldset
                    eeFieldset.Controls.Add(formPanel0)
                    eeFieldset.Controls.Add(formPanel1)
                    eeFieldset.Controls.Add(formPanel2)
                    eeFieldset.Controls.Add(formPanel3)
                    eeFieldset.Controls.Add(formPanel4)
                    If examinerType = "unit" Then
                        eeFieldset.Controls.Add(formPanel5)
                        eeFieldset.Controls.Add(formPanel6)
                        eeFieldset.Controls.Add(formPanel7)
                    End If
                    eeFieldset.Controls.Add(confirmPanel)


                    'add fieldset to container div
                    outputDiv.Controls.Add(eeFieldset)



                    '----------------------------------------- CREATE PANEL ELEMENTS ------------------
                    Dim numberofpanels As Int32
                    If examinerType = "unit" Then
                        numberofpanels = numberofunitpanels
                    ElseIf examinerType = "award" Then
                        numberofpanels = numberofawardpanels
                    End If


                    Try
                        'create panel elements for all panels
                        currentpanel = -1
                        For panelno As Integer = 0 To numberofpanels

                            currentpanel = currentpanel + 1
                            createPanelElements(examinerType, panelno, numberofpanels)

                            '+++++++++++++++ creating list of panels for navigation ------------
                            Dim panelnavlistitem As New LI
                            panelnavlistitem.CssClass = "navigationlistitem"
                            panelnavlistitem.InnerHTML = currentpanel.ToString
                            panelnavigationlist.Controls.Add(panelnavlistitem)

                            If currentpanel = 0 Then
                                panelnavlistitem.InnerHTML = "Start"
                                panelnavlistitem.CssClass = "startnavigationlistitem"
                            End If

                        Next
                    Catch ex As Exception
                        response.Write("ERROR - ExternalExaminers ExternalExaminersForm_Init: " & ex.Message())
                    End Try

                    '---------------------------------------------------------------------------------------------

                    If Page.IsPostBack Then
                        currentpanel = session("currentpanel")
                    Else
                        session("currentpanel") = 0
                        currentpanel = 0
                    End If


                    nextButton.Visible = False
                    prevButton.Visible = False
                    saveButton.Visible = True
                    submitButton.Visible = True

                    updateButtonDiv.CssClass = "sys_form-buttons image-align-left"
                    outputDiv.Controls.Add(updateButtonDiv)
                    updateButtonDiv.Controls.Add(prevButton)
                    updateButtonDiv.Controls.Add(nextButton)
                    updateButtonDiv.Controls.Add(submitButton)

                    Dim myHiddenPanelId As New HiddenField
                    myHiddenPanelId.ID = "txtPanelId"
                    myHiddenPanelId.Value = "0"
                    updateButtonDiv.Controls.Add(myHiddenPanelId)

                    Dim savebuttondiv2 As New Div
                    savebuttondiv2.CssClass = "image-align-right"
                    savebuttondiv2.Attributes.Add("style", "text-align:right")
                    savebuttondiv2.Controls.Add(saveButton2)
                    savebuttondiv2.Controls.Add(savenotificationPanel2)
                    outputDiv.Controls.Add(savebuttondiv2)

                    If Not Page.IsPostBack Then
                        formPanel0.Visible = True
                        formPanel1.Visible = True
                        formPanel2.Visible = True
                        formPanel3.Visible = True
                        formPanel4.Visible = True
                        formPanel5.Visible = True
                        formPanel6.Visible = True
                        formPanel7.Visible = True
                        confirmPanel.Visible = True
                    Else
                        '   response.Write("is postback<br />")
                    End If
                Catch ex As Exception
                    response.Write("ERROR: ExternalExaminersFormUnit_Init 2 " + ex.ToString)
                End Try
            End If

        End Sub


        Public Function getreportdate()
            Dim reportdate As String = ""
            Try

                Dim todaysdate As Date = Today
                Dim theyear As Int32 = Year(todaysdate)

                Dim startdate As String = "01/02/" + theyear.ToString
                Dim enddate As String = "31/01/" + (theyear + 1).ToString

                If todaysdate < startdate Then
                    reportdate = (theyear - 2).ToString + "/" + Right((theyear - 1.ToString), 2)
                ElseIf todaysdate > enddate Then
                    reportdate = (theyear.ToString) + "/" + Right((theyear + 1.ToString), 2)
                Else
                    reportdate = (theyear - 1).ToString + "/" + Right((theyear.ToString), 2)
                End If

            Catch ex As Exception
                response.Write("ERROR: ExternalExaminers getreportdate " + ex.ToString)
            End Try
            Return reportdate
        End Function

        Private Sub createPanelElements(ByVal examinertype As String, ByVal currentpanel As Int32, ByVal numberofpanels As Int32)

            Try
                'if panel 0
                If currentpanel = 0 Then

                    nextButton.Visible = True
                    nextButton.Text = "Start"

                    Dim examinertypename As String = ""

                    If examinertype = "unit" Then
                        examinertypename = "Unit"
                    ElseIf examinertype = "award" Then
                        examinertypename = "Progression and award"
                    End If

                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Clear()
                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(New Heading(2, examinertypename + " external examiner " + reportyear))
                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(New P(examinertypename + " external examiner reports will be received and considered by Academic Services. They will also be considered formally as part of the course review process. You will normally receive a response to your report within six weeks of submission. A summary of themes emerging from external examiner reports is presented to the University’s Academic Board."))
                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(New P("External examiner reports are made available in full to students. They also constitute recorded information held by the University and are therefore open to disclosure if requested under the Freedom of Information Act. "))
                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(New P("The University will take very seriously any negative answers and will work with the course teams concerned to address the issues raised in a supportive and developmental way."))
                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(New P("<strong>Please note: Individual students or members of staff should not be identified in the report.</strong>"))
                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(New P("You will be prompted to use free text boxes within your report in order to provide qualitative feedback. This will be used to disseminate good practice and identify areas for enhancement. "))
                    eeRecord = New Record
                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(New Heading(2, "General information"))
                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(New P("This information is held in our system. If any information is incorrect then please contact <a href=""mailto:as.externalexaminers@solent.ac.uk"">as.externalexaminers@solent.ac.uk</a>"))

                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(eeRecord.createRecord(examinerid, False, "form"))
                Else
                    nextButton.Text = "Next"
                End If

                'add progress bar to all panels except 0
                If currentpanel > 0 And currentpanel < numberofpanels + 1 Then

                    nextButton.Visible = True

                    'Dim sectiondiv As New Div
                    'sectiondiv.CssClass = "section"
                    ' sectiondiv.InnerHTML = "Section " + currentpanel.ToString + " of " + numberofpanels.ToString
                    ' Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(sectiondiv)

                    'create unit questions
                    '---------------------------------------------------------------------------------------------
                    If examinertype = "unit" Then
                        If currentpanel = 1 Then

                            Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(New Heading(2, "Section 1 - Standards"))

                            Dim rowdiv As New Div
                            rowdiv.CssClass = "formrow unitheading"

                            Dim numberdiv As New Div
                            numberdiv.CssClass = "number"
                            numberdiv.InnerHTML = "1.1"
                            rowdiv.Controls.Add(numberdiv)

                            Dim textdiv As New Div
                            textdiv.CssClass = "text"
                            textdiv.InnerHTML = "Unit standards"
                            rowdiv.Controls.Add(textdiv)

                            Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(rowdiv)

                            createUnitQuestions(examinerid)

                        End If
                    End If

                    '---------------------------------------------------------------------------------------------

                    ' response.Write("isSaved = " + isSaved.ToString)
                    'response.Write("in createpanelelements line 304 status = " + status + "<br />")

                    'create rest of the questions and answers if there are any
                    If status = "submitted" Or status = "saved" Then
                        createQuestionsAndAnswers(examinerid, examinertype, currentpanel)
                    Else
                        createQuestionsAndAnswers("null", examinertype, currentpanel)
                    End If


                End If

                'if last panel show submit button
                If currentpanel = numberofpanels Then
                    nextButton.Visible = False
                    submitButton.Visible = True
                End If

            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers CreatePanelElements: " & ex.Message())

            End Try

        End Sub

        'Protected Sub nextButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        '    formPanel0.Visible = False
        '    formPanel1.Visible = False
        '    formPanel2.Visible = False
        '    formPanel3.Visible = False
        '    formPanel4.Visible = False
        '    formPanel5.Visible = False
        '    formPanel6.Visible = False
        '    formPanel7.Visible = False
        '    confirmPanel.Visible = False

        '    'increment current panel
        '    currentpanel = currentpanel + 1
        '    session("currentpanel") = currentpanel
        '    response.Write("nextbutton_click increment panel to " + currentpanel.ToString+ "<br />")

        '    'make next panel visible
        '    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Visible = True

        '    'create panel elements for next panel
        '    'createPanelElements("unit", currentpanel)

        '    'add previous button
        '    prevButton.Visible = True

        'End Sub

        'Protected Sub prevButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        '    formPanel0.Visible = False
        '    formPanel1.Visible = False
        '    formPanel2.Visible = False
        '    formPanel3.Visible = False
        '    formPanel4.Visible = False
        '    formPanel5.Visible = False
        '    formPanel6.Visible = False
        '    formPanel7.Visible = False
        '    confirmPanel.Visible = False

        '    'increment current panel
        '    currentpanel = currentpanel - 1
        '    session("currentpanel") = currentpanel

        '    'make next panel visible
        '    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Visible = True

        '    'create panel elements
        '    'createPanelElements("unit", currentpanel)

        '    If currentpanel = 0 Then
        '        prevButton.Visible = False
        '    End If
        'End Sub

        'Private Sub createQuestions(ByVal formtype As String, ByVal panelIn As String)

        '    Try
        '        Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
        '        myDatabaseUtil.addParameter("@type", "unit")
        '        myDatabaseUtil.addParameter("@panel", panelIn)
        '        Dim myDataReader As SqlDataReader = myDatabaseUtil.getSqlDataReader("storproc_getreportquestions")

        '        If myDataReader.HasRows Then

        '            While myDataReader.Read()

        '                Dim rowDiv As New Div()
        '                rowDiv.CssClass = "formrow"

        '                Dim headingbefore As String = ""
        '                Dim questionid As String = ""
        '                Dim number As String = ""
        '                Dim text As String = ""
        '                Dim yes As String = ""
        '                Dim no As String = ""
        '                Dim na As String = ""
        '                Dim comment As String = ""
        '                Dim radiocomment As String = ""
        '                Dim display As String = ""
        '                Dim panel As String = ""
        '                Dim lastquestioninpanel As String = ""

        '                If Not myDataReader("headingbefore") Is DBNull.Value Then
        '                    headingbefore = myDataReader("headingbefore")
        '                End If

        '                If Not myDataReader("number") Is DBNull.Value Then
        '                    number = myDataReader("number")
        '                End If

        '                If Not myDataReader("questionid") Is DBNull.Value Then
        '                    questionid = myDataReader("questionid")
        '                End If

        '                If Not myDataReader("text") Is DBNull.Value Then
        '                    text = myDataReader("text")
        '                End If

        '                If Not myDataReader("yes") Is DBNull.Value Then
        '                    yes = myDataReader("yes")
        '                End If

        '                If Not myDataReader("no") Is DBNull.Value Then
        '                    no = myDataReader("no")
        '                End If

        '                If Not myDataReader("na") Is DBNull.Value Then
        '                    na = myDataReader("na")
        '                End If

        '                If Not myDataReader("comment") Is DBNull.Value Then
        '                    comment = myDataReader("comment")
        '                End If

        '                If Not myDataReader("radiocomment") Is DBNull.Value Then
        '                    radiocomment = myDataReader("radiocomment")
        '                End If

        '                If Not myDataReader("display") Is DBNull.Value Then
        '                    display = myDataReader("display")
        '                End If

        '                If Not myDataReader("panel") Is DBNull.Value Then
        '                    panel = myDataReader("panel")
        '                End If

        '                If Not myDataReader("lastquestioninpanel") Is DBNull.Value Then
        '                    lastquestioninpanel = myDataReader("lastquestioninpanel")
        '                End If

        '                If Not headingbefore = "false" Then
        '                    rowDiv.Controls.Add(New Div(headingbefore))
        '                End If

        '                If display = True Then

        '                    Dim numberDiv As New Div()

        '                    If number.Length > 0 Then
        '                        numberDiv.CssClass = "number"
        '                        numberDiv.InnerHTML = number
        '                    End If

        '                    Dim textDiv As New Div()
        '                    textDiv.CssClass = "text"
        '                    textDiv.InnerHTML = text

        '                    rowDiv.Controls.Add(numberDiv)
        '                    rowDiv.Controls.Add(textDiv)

        '                    If yes = True Then
        '                        Dim RadioButton As New Radio
        '                        RadioButton.ID = "yes_" + questionid
        '                        RadioButton.Name = questionid
        '                        RadioButton.Attributes.Add("value", "yes")

        '                        Dim yesspan As New Span
        '                        yesspan.CssClass = "radiospan"
        '                        yesspan.InnerHTML = "Yes: "
        '                        rowDiv.Controls.Add(yesspan)
        '                        rowDiv.Controls.Add(RadioButton)
        '                    End If

        '                    If no = True Then
        '                        Dim RadioButton As New Radio
        '                        RadioButton.ID = "no_" + questionid
        '                        RadioButton.Name = questionid
        '                        RadioButton.Attributes.Add("value", "no")

        '                        Dim nospan As New Span
        '                        nospan.CssClass = "radiospan"
        '                        nospan.InnerHTML = "No: "
        '                        rowDiv.Controls.Add(nospan)
        '                        rowDiv.Controls.Add(RadioButton)
        '                    End If

        '                    If na = True Then
        '                        Dim RadioButton As New Radio
        '                        RadioButton.ID = "na_" + questionid
        '                        RadioButton.Name = questionid
        '                        RadioButton.Attributes.Add("value", "na")

        '                        Dim naspan As New Span
        '                        naspan.CssClass = "radiospan"
        '                        naspan.InnerHTML = "N/A: "
        '                        rowDiv.Controls.Add(naspan)
        '                        rowDiv.Controls.Add(RadioButton)
        '                    End If

        '                    If comment = True Then

        '                        If radiocomment = True Then 'radio comment
        '                            rowDiv.Controls.Add(getRadioComment(questionid))
        '                        Else 'normal comment

        '                            Dim commentdiv As New Div
        '                            Dim commentbox As New TextBox
        '                            commentbox.ID = questionid
        '                            commentbox.ValidationGroup = "externalexaminersformvalidationgroup"
        '                            commentbox.CssClass = "commentbox"

        '                            commentdiv.CssClass = "commentspan"

        '                            rowDiv.Controls.Add(commentdiv)
        '                            rowDiv.Controls.Add(commentbox)

        '                        End If

        '                    End If


        '                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(rowDiv)
        '                    'eeFieldset.Controls.Add(rowDiv)
        '                    'Me.FindControl("externalexaminersformpanel" + panel).Controls.Add(eeFieldset)

        '                End If

        '            End While
        '        Else
        '            ' response.Write("datareader1 has no rows")
        '            eeFieldset.Controls.Add(New P("A problem has occurred."))
        '        End If

        '        ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)

        '    Catch ex As Exception
        '        response.Write("ERROR - ExternalExaminers CreateQuestions: " & ex.Message())

        '    End Try

        'End Sub

        Public Sub createQuestionsAndAnswers(ByVal examinerid As String, ByVal examinertype As String, ByVal panelIn As String)

            Dim myDataReader As SqlDataReader = Nothing
            Try
                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                myDatabaseUtil.addParameter("@examinerid", examinerid)
                myDatabaseUtil.addParameter("@type", examinertype)
                myDatabaseUtil.addParameter("@panel", panelIn)
                myDataReader = myDatabaseUtil.getSqlDataReader("storproc_getreportquestionsandanswers")

                If myDataReader.HasRows Then

                    While myDataReader.Read()
                        Dim rowDiv As New Div()
                        rowDiv.CssClass = "formrow"

                        Dim headingbefore As String = ""

                        Dim number As String = ""
                        Dim text As String = ""
                        Dim yes As String = ""
                        Dim no As String = ""
                        Dim na As String = ""
                        Dim comment As String = ""
                        Dim radiocomment As String = ""
                        Dim display As String = ""
                        Dim panel As String = ""
                        Dim lastquestioninpanel As String = ""
                        Dim validate As Boolean = False
                        Dim validationmessage As String = ""
                        Dim questionresponse As String = ""

                        If Not myDataReader("headingbefore") Is DBNull.Value Then
                            headingbefore = myDataReader("headingbefore")
                        End If

                        If Not myDataReader("number") Is DBNull.Value Then
                            number = myDataReader("number")
                        End If

                        If Not myDataReader("questionid") Is DBNull.Value Then
                            questionid = myDataReader("questionid")
                        End If

                        If Not myDataReader("text") Is DBNull.Value Then
                            text = myDataReader("text")
                        End If

                        If Not myDataReader("yes") Is DBNull.Value Then
                            yes = myDataReader("yes")
                        End If

                        If Not myDataReader("no") Is DBNull.Value Then
                            no = myDataReader("no")
                        End If

                        If Not myDataReader("na") Is DBNull.Value Then
                            na = myDataReader("na")
                        End If

                        If Not myDataReader("comment") Is DBNull.Value Then
                            comment = myDataReader("comment")
                        End If

                        If Not myDataReader("radiocomment") Is DBNull.Value Then
                            radiocomment = myDataReader("radiocomment")
                        End If

                        If Not myDataReader("display") Is DBNull.Value Then
                            display = myDataReader("display")
                        End If

                        If Not myDataReader("panel") Is DBNull.Value Then
                            panel = myDataReader("panel")
                        End If

                        If Not myDataReader("lastquestioninpanel") Is DBNull.Value Then
                            lastquestioninpanel = myDataReader("lastquestioninpanel")
                        End If

                        If Not myDataReader("validate") Is DBNull.Value Then
                            validate = myDataReader("validate")
                        End If

                        If Not myDataReader("validationmessage") Is DBNull.Value Then
                            validationmessage = myDataReader("validationmessage")
                        End If


                        If Not examinerid = "null" Then
                            If Not myDataReader("response") Is DBNull.Value Then
                                questionresponse = myDataReader("response")
                            End If
                        End If

                        'display the question heading if needed
                        If Not headingbefore = "false" Then
                            rowDiv.Controls.Add(New Div(headingbefore))
                        End If

                        'response.Write("question id = " + questionid + " and response = " + questionresponse + "<br />")

                        If display = True Then

                            Dim numberDiv As New Div()

                            ' If number.Length > 0 Then
                            numberDiv.CssClass = "number"

                            If validate = True And number.Length > 0 Then
                                number = number + " *"
                            End If

                            numberDiv.InnerHTML = number
                            ' End If

                            Dim textDiv As New Div()
                            textDiv.CssClass = "text"
                            textDiv.InnerHTML = text

                            rowDiv.Controls.Add(numberDiv)
                            rowDiv.Controls.Add(textDiv)


                            Dim radiobuttonDiv As New Div
                            radiobuttonDiv.CssClass = "radiobuttoncontainer"

                            If yes = True Then

                                rowDiv.Controls.Add(radiobuttonDiv)
                                Dim RadioButtonyes As New Radio

                                isExistsYesRadioButton = True
                                RadioButtonyes.ID = "yes_" + questionid
                                RadioButtonyes.Name = questionid
                                RadioButtonyes.Attributes.Add("value", "yes")

                                If questionresponse = "yes" Then
                                    RadioButtonyes.Checked = True
                                End If

                                Dim yesspan As New Span
                                yesspan.CssClass = "radiospan"
                                yesspan.InnerHTML = "Yes: "
                                radiobuttonDiv.Controls.Add(yesspan)
                                radiobuttonDiv.Controls.Add(RadioButtonyes)

                                If validate = True Then

                                    'if radio buttons
                                    If isExistsYesRadioButton Then

                                        Dim customvalidator1 As New CustomValidator
                                        customvalidator1.ID = "cfv" + questionid
                                        'response.Write("hello???" & customvalidator1.ID & "<br />")
                                        customvalidator1.CssClass = "error validationerrormessage"
                                        customvalidator1.EnableClientScript = False
                                        customvalidator1.Display = ValidatorDisplay.Dynamic
                                        customvalidator1.ValidationGroup = "externalexaminersvalidationgroup"
                                        customvalidator1.ErrorMessage = validationmessage
                                        AddHandler customvalidator1.ServerValidate, AddressOf radiobuttonvalidator_validate
                                        rowDiv.Controls.Add(customvalidator1)
                                    End If

                                End If
                                isExistsYesRadioButton = False
                            End If

                            If no = True Then
                                ' isExistsNoRadioButton = True
                                Dim RadioButtonno As New Radio

                                RadioButtonno.ID = "no_" + questionid
                                RadioButtonno.Name = questionid
                                RadioButtonno.Attributes.Add("value", "no")

                                If questionresponse = "no" Then
                                    RadioButtonno.Checked = True
                                End If

                                Dim nospan As New Span
                                nospan.CssClass = "radiospan"
                                nospan.InnerHTML = "No: "
                                radiobuttonDiv.Controls.Add(nospan)
                                radiobuttonDiv.Controls.Add(RadioButtonno)
                            End If

                            If na = True Then
                                '  isExistsNaRadioButton = True
                                Dim RadioButtonna As New Radio
                                RadioButtonna.ID = "na_" + questionid
                                RadioButtonna.Name = questionid
                                RadioButtonna.Attributes.Add("value", "na")

                                If questionresponse = "na" Then
                                    RadioButtonna.Checked = True
                                End If

                                Dim naspan As New Span
                                naspan.CssClass = "radiospan"
                                naspan.InnerHTML = "N/A: "
                                radiobuttonDiv.Controls.Add(naspan)
                                radiobuttonDiv.Controls.Add(RadioButtonna)
                            End If

                            If comment = "True" Then

                                If radiocomment = True Then 'normal comment
                                    rowDiv.Controls.Add(getRadioComment(questionid, questionresponse))
                                Else

                                    Dim commentdiv As New Div
                                    Dim commentbox As New TextBox
                                    commentbox.ID = questionid
                                    commentbox.ValidationGroup = "externalexaminersvalidationgroup"
                                    commentbox.CssClass = "commentbox"
                                    commentbox.TextMode = TextBoxMode.MultiLine
                                    commentbox.Text = questionresponse

                                    commentdiv.CssClass = "commentspan"

                                    rowDiv.Controls.Add(commentdiv)
                                    rowDiv.Controls.Add(commentbox)

                                    If validate = True Then
                                        Dim requiredField As New RequiredFieldValidator
                                        requiredField.ID = "rfv" + questionid
                                        requiredField.CssClass = "error validationerrormessage"
                                        requiredField.EnableClientScript = False
                                        requiredField.ControlToValidate = questionid
                                        requiredField.Display = ValidatorDisplay.Dynamic
                                        requiredField.ValidationGroup = "externalexaminersvalidationgroup"
                                        requiredField.ErrorMessage = validationmessage


                                        rowDiv.Controls.Add(requiredField)
                                    End If
                                End If

                            End If


                            Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(rowDiv)

                        End If

                    End While
                Else
                    eeFieldset.Controls.Add(New P("A problem has occurred."))
                End If
            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers CreateQuestionsandAnswers: " & ex.Message())
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try

        End Sub


        Protected Sub radiobuttonvalidatorunit_validate(source As Object, args As ServerValidateEventArgs)
            Dim radioid = source.id.replace("cfvf", "")
            Dim RadioQuestiona = "a1" + radioid
            Dim RadioQuestionb = "b1" + radioid
            Dim RadioQuestionc = "c1" + radioid

            ' response.Write(RadioQuestiona + " = " + request.Form(RadioQuestiona + "<br />"))
            'response.Write(RadioQuestionb + " = " + request.Form(RadioQuestionb + "<br />"))
            'response.Write(RadioQuestionc + " = " + request.Form(RadioQuestionc + "<br />"))

            If String.IsNullOrEmpty(request.Form(RadioQuestiona)) Or String.IsNullOrEmpty(request.Form(RadioQuestionb)) Or String.IsNullOrEmpty(request.Form(RadioQuestionc)) Then
                '  response.Write("this is happening<br />")
                args.IsValid = False
            Else
                args.IsValid = True
            End If

        End Sub

        Protected Sub radiobuttonvalidator_validate(source As Object, args As ServerValidateEventArgs)
            Dim currentRadioQuestion = source.id.replace("cfv", "")
            '  response.Write("currentRadioQuestion = " + currentRadioQuestion + "<br />")
            If String.IsNullOrEmpty(request.Form(currentRadioQuestion)) Then

                args.IsValid = False
            Else
                args.IsValid = True
            End If

        End Sub

        Public Function getQuestionIdArray(ByVal examinertype As String)

            Dim questionid As String
            Dim questionidArray As New ArrayList
            Dim myDataReader As SqlDataReader = Nothing
            Try
                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                myDatabaseUtil.addParameter("@type", examinertype)
                myDatabaseUtil.addParameter("@panel", 999)
                myDataReader = myDatabaseUtil.getSqlDataReader("storproc_getreportquestions")

                If myDataReader.HasRows Then

                    While myDataReader.Read()

                        If Not myDataReader("questionid") Is DBNull.Value Then
                            questionid = myDataReader("questionid")
                            questionidArray.Add(questionid)

                        End If
                    End While

                End If

                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)

                Return questionidArray

            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers getQuestionIdArray: " & ex.Message())
            Finally

                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try

        End Function

        Public Function getRadioComment(ByVal questionid As String, Optional ByVal theresponse As String = "")
            Dim myDataReader As SqlDataReader = Nothing

            ' questionid = questionid + "_comment"
            Dim validate As Boolean = False
            Dim validationmessage As String = ""

            Dim questionidtodatabase As String = questionid + "_comment"
            Try
                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                myDatabaseUtil.addParameter("@questionid", questionidtodatabase)
                myDatabaseUtil.addParameter("@examinerid", examinerid)
                myDataReader = myDatabaseUtil.getSqlDataReader("storproc_getradiocomment")

                Dim myDiv As New Div
                myDiv.CssClass = "radiocommentdiv"
                myDiv.InnerHTML = ""

                If myDataReader.HasRows Then
                    ' response.Write("radio comment " + questionid + "has rows<br />")
                    While myDataReader.Read()

                        If Not myDataReader("validate") Is DBNull.Value Then
                            validate = myDataReader("validate")
                        End If

                        If Not myDataReader("validationmessage") Is DBNull.Value Then
                            validationmessage = myDataReader("validationmessage")
                        End If

                        Dim response As String = ""
                        If Not myDataReader("response") Is DBNull.Value Then
                            response = myDataReader("response")
                        End If
                        ' response.write("response

                        Dim commentspan As New Span
                        commentspan.CssClass = "radiocommentspan"
                        commentspan.InnerHTML = "Examiner comment:"

                        Dim commentbox As New TextBox

                        commentbox.ID = questionid + "_comment"
                        commentbox.CssClass = "radiocommentbox"
                        commentbox.TextMode = TextBoxMode.MultiLine
                        commentbox.Text = response
                        'theresponse
                        commentbox.ValidationGroup = "externalexaminersvalidationgroup"

                        myDiv.Controls.Add(commentspan)
                        myDiv.Controls.Add(New BR)
                        myDiv.Controls.Add(commentbox)


                        If validate = True Then
                            Dim requiredField As New RequiredFieldValidator
                            requiredField.ID = "rfv" + questionid + "_comment"
                            requiredField.CssClass = "error"
                            requiredField.EnableClientScript = False
                            requiredField.ControlToValidate = questionid + "_comment"
                            requiredField.Display = ValidatorDisplay.Dynamic
                            requiredField.ValidationGroup = "externalexaminersvalidationgroup"
                            requiredField.ErrorMessage = validationmessage

                            myDiv.Controls.Add(requiredField)
                        End If

                    End While
                    '  Else
                    '  response.Write("radio comment " + questionid + " has NO rows<br />")
                End If

                Return myDiv
            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers getradiocomment: " & ex.Message())
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try
        End Function

        Public Function getRadioCommentWord(ByVal questionid As String, Optional ByVal theresponse As String = "")
            Dim myDataReader As SqlDataReader = Nothing
            Dim questionidtodatabase As String = questionid + "_comment"

            Try
                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                myDatabaseUtil.addParameter("@questionid", questionidtodatabase)
                myDatabaseUtil.addParameter("@examinerid", examinerid)
                myDataReader = myDatabaseUtil.getSqlDataReader("storproc_getradiocomment")

                Dim answer As String = ""
                If myDataReader.HasRows Then
                    While myDataReader.Read()

                        If Not myDataReader("response") Is DBNull.Value Then
                            answer = myDataReader("response")
                        End If
                    End While
             
                End If

                Return answer
            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers getradiocommentWord: " & ex.Message())
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try
        End Function

        Public Sub createUnitQuestions(ByVal examineridIn As String)

            '--check if they have unit responses -----------------------------------------------------------------------------

            ' response.Write("in createunitquestions examinerid = " + examineridIn + "<br />")
            Dim isComplete As Boolean = False
            Dim myDataReader As SqlDataReader = Nothing
            Dim myDataReader2 As SqlDataReader = Nothing
            Try
                'check if the examiner has completed their unit questions
                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                myDatabaseUtil.addParameter("@examinerid", examineridIn)
                myDatabaseUtil.addParameter("@reportyear", reportyear)
                myDataReader = myDatabaseUtil.getSqlDataReader("storproc_checkforunitresponses")

                'if they have
                If myDataReader.HasRows Then
                    isComplete = True
                Else 'they havent
                    isComplete = False
                End If

            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers CreateUnitQuestions1: " & ex.Message())

            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try


            '--get unit responses -----------------------------------------------------------------------------

            Try
                'then run query again with correct parameter to pull back questions/questions and results
                Dim myDatabaseUtil2 As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                myDatabaseUtil2.addParameter("@examinerid", examineridIn)
                myDatabaseUtil2.addParameter("@completed", isComplete.ToString)
                myDatabaseUtil2.addParameter("@reportyear", reportyear)

                ' response.Write(examineridIn + ", " + isComplete.ToString + ", " + reportyear + "<br />")

                myDataReader2 = myDatabaseUtil2.getSqlDataReader("storproc_getunits")

                If myDataReader2.HasRows Then
                    '    response.Write("has rows<br />")
                    Dim unittable As New Table

                    Dim tableheader As New TableHeaderRow

                    Dim tableheadercell1 As New TableHeaderCell
                    Dim tableheadercell2 As New TableHeaderCell
                    Dim tableheadercell3 As New TableHeaderCell
                    Dim tableheadercell4 As New TableHeaderCell

                    tableheadercell2.Text = "Were the standards set for the units appropriate for their level?"
                    tableheadercell2.ColumnSpan = 2
                    tableheadercell3.Text = "Were the standards of student performance comparable with similar programmes or subjects in other UK institutions with which you are familiar?"
                    tableheadercell3.ColumnSpan = 2
                    tableheadercell4.Text = "Were you sent copies of all assessment tasks prior to their release to students?"
                    tableheadercell4.ColumnSpan = 2

                    tableheader.Controls.Add(tableheadercell1)
                    tableheader.Controls.Add(tableheadercell2)
                    tableheader.Controls.Add(tableheadercell3)
                    tableheader.Controls.Add(tableheadercell4)
                    unittable.Controls.Add(tableheader)

                    Dim tablerowsecond As New TableRow

                    Dim tablecellsecond1 As New TableHeaderCell
                    Dim tablecellsecond2 As New TableHeaderCell
                    Dim tablecellsecond3 As New TableHeaderCell
                    Dim tablecellsecond4 As New TableHeaderCell
                    Dim tablecellsecond5 As New TableHeaderCell
                    Dim tablecellsecond6 As New TableHeaderCell
                    Dim tablecellsecond7 As New TableHeaderCell

                    tablecellsecond1.Text = "Unit information"
                    tablecellsecond2.Text = "Yes"
                    tablecellsecond3.Text = "No"
                    tablecellsecond4.Text = "Yes"
                    tablecellsecond5.Text = "No"
                    tablecellsecond6.Text = "Yes"
                    tablecellsecond7.Text = "No"

                    tablerowsecond.Controls.Add(tablecellsecond1)
                    tablerowsecond.Controls.Add(tablecellsecond2)
                    tablerowsecond.Controls.Add(tablecellsecond3)
                    tablerowsecond.Controls.Add(tablecellsecond4)
                    tablerowsecond.Controls.Add(tablecellsecond5)
                    tablerowsecond.Controls.Add(tablecellsecond6)
                    tablerowsecond.Controls.Add(tablecellsecond7)

                    unittable.Controls.Add(tablerowsecond)

                    Dim iRow As Integer = 0
                    Dim rowColour As String = "#fff"

                    While myDataReader2.Read()

                        If iRow Mod 2 = 0 Then
                            rowColour = "#fff"
                        Else
                            rowColour = "#f9f9f9"
                        End If

                        Dim unitcode As String = ""
                        Dim unitid As String = ""
                        Dim unitname As String = ""
                        Dim unitlevel As String = ""
                        Dim academicsession As String = ""

                        If Not myDataReader2("unitcode") Is DBNull.Value Then
                            unitcode = myDataReader2("unitcode")
                        End If

                        If Not myDataReader2("unitid") Is DBNull.Value Then
                            unitid = myDataReader2("unitid")
                        End If

                        If Not myDataReader2("unitname") Is DBNull.Value Then
                            unitname = myDataReader2("unitname")
                        End If

                        If Not myDataReader2("unitlevel") Is DBNull.Value Then
                            unitlevel = myDataReader2("unitlevel")
                        End If

                        If Not myDataReader2("academic_session") Is DBNull.Value Then
                            academicsession = myDataReader2("academic_session")
                        End If


                        Dim a1_1_response As String = ""
                        Dim b1_1_response As String = ""
                        Dim c1_1_response As String = ""

                        If isComplete = True Then
                            If Not myDataReader2("a1_1") Is DBNull.Value Then
                                a1_1_response = myDataReader2("a1_1")
                            End If
                            If Not myDataReader2("b1_1") Is DBNull.Value Then
                                b1_1_response = myDataReader2("b1_1")
                            End If
                            If Not myDataReader2("c1_1") Is DBNull.Value Then
                                c1_1_response = myDataReader2("c1_1")
                            End If
                        End If


                        Dim tablerow As New TableRow
                        tablerow.Attributes.Add("style", "background-color: " + rowColour)

                        Dim tablecell1 As New TableCell
                        Dim tablecell2 As New TableCell
                        Dim tablecell3 As New TableCell
                        Dim tablecell4 As New TableCell
                        Dim tablecell5 As New TableCell
                        Dim tablecell6 As New TableCell
                        Dim tablecell7 As New TableCell

                        Dim radiobutton1 As New Radio
                        Dim radiobutton2 As New Radio
                        Dim radiobutton3 As New Radio
                        Dim radiobutton4 As New Radio
                        Dim radiobutton5 As New Radio
                        Dim radiobutton6 As New Radio

                        radiobutton1.ID = "a1_1_" + unitid
                        radiobutton1.Name = "a1" + unitid

                        If a1_1_response = "yes" Then
                            radiobutton1.Checked = True
                        End If

                        radiobutton2.ID = "a1_0_" + unitid
                        radiobutton2.Name = "a1" + unitid

                        If a1_1_response = "no" Then
                            radiobutton2.Checked = True
                        End If

                        radiobutton3.ID = "b1_1_" + unitid
                        radiobutton3.Name = "b1" + unitid

                        If b1_1_response = "yes" Then
                            radiobutton3.Checked = True
                        End If

                        radiobutton4.ID = "b1_0_" + unitid
                        radiobutton4.Name = "b1" + unitid

                        If b1_1_response = "no" Then
                            radiobutton4.Checked = True
                        End If

                        radiobutton5.ID = "c1_1_" + unitid
                        radiobutton5.Name = "c1" + unitid

                        If c1_1_response = "yes" Then
                            radiobutton5.Checked = True
                        End If

                        radiobutton6.ID = "c1_0_" + unitid
                        radiobutton6.Name = "c1" + unitid

                        If c1_1_response = "no" Then
                            radiobutton6.Checked = True
                        End If

                        Dim customvalidatorunit As New CustomValidator
                        customvalidatorunit.ID = "cfvf" + unitid
                        customvalidatorunit.CssClass = "error"
                        customvalidatorunit.Display = ValidatorDisplay.Dynamic
                        customvalidatorunit.ValidationGroup = "externalexaminersvalidationgroup"
                        customvalidatorunit.ErrorMessage = "Response for unit '" + unitname + "' is required"
                        AddHandler customvalidatorunit.ServerValidate, AddressOf radiobuttonvalidatorunit_validate

                        tablecell1.Controls.Add(New Span(unitname + " (" + unitcode + ") " + unitlevel + " - " + academicsession))
                        ' tablecell1.Controls.Add(customvalidatorunit)
                        tablecell2.Controls.Add(radiobutton1)
                        tablecell3.Controls.Add(radiobutton2)
                        tablecell4.Controls.Add(radiobutton3)
                        tablecell5.Controls.Add(radiobutton4)
                        tablecell6.Controls.Add(radiobutton5)
                        tablecell7.Controls.Add(radiobutton6)

                        tablerow.Controls.Add(tablecell1)
                        tablerow.Controls.Add(tablecell2)
                        tablerow.Controls.Add(tablecell3)
                        tablerow.Controls.Add(tablecell4)
                        tablerow.Controls.Add(tablecell5)
                        tablerow.Controls.Add(tablecell6)
                        tablerow.Controls.Add(tablecell7)

                        unittable.Controls.Add(tablerow)



                        iRow = iRow + 1


                    End While

                    Me.FindControl("externalexaminersformpanel" + currentpanel.ToString).Controls.Add(unittable)
                Else
                    '  response.Write("no rows<br />")
                End If

            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers CreateUnitQuestions2: " & ex.Message())
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader2)

            End Try


        End Sub

        Protected Sub submitButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            Page.Validate()

            If Page.IsValid() Then
                saveData("submitted")
                status = "submitted"

                '  response.Write("submitbutton_click status = " + status + "<br />")

                Dim confirmDiv As New Div
                confirmDiv.Controls.Add(New Heading(3, "Thank you, your feedback has been submitted"))
                confirmDiv.Controls.Add(New P("Please use the 'Download form as PDF' link below if you wish to retain a copy of your report."))
                confirmDiv.Controls.Add(New P("<strong>Please note: </strong>The PDF download may take a few minutes to complete."))
                confirmDiv.Controls.Add(New P("<a class=""pdf-download-link"" href=""/portal-apps/external-examiners/feedback/form.aspx?id=" + examinerid + "&type=" + examinerType + "&print=true"">Download form as PDF</a>"))
                confirmDiv.Controls.Add(New P(Nothing))
                confirmDiv.Controls.Add(New P("If you wish to make changes to your report you can re-edit your report using the link below which was sent to you in your original email. "))
                confirmDiv.Controls.Add(New P("<a class=""read-more-link"" href=""/portal-apps/external-examiners/feedback/form.aspx?id=" + examinerid + "&type=" + examinerType + """>Go to your annual report form</a>"))
               confirmPanel.Controls.Add(confirmDiv)

                prevButton.Visible = False
                submitButton.Visible = False
                toggleViewButton.Visible = False
                panelnavigation.Visible = False
                saveButton.Visible = False
                saveButton2.Visible = False
                saveExplanation.Visible = False
                savenotificationPanel.Visible = False
                savenotificationPanel2.Visible = False
                updateButtonDiv.Visible = False
                formPanel0.Visible = False
                formPanel1.Visible = False
                formPanel2.Visible = False
                formPanel3.Visible = False
                formPanel4.Visible = False
                formPanel5.Visible = False
                formPanel6.Visible = False
                formPanel7.Visible = False
                confirmPanel.Visible = True
            End If

        End Sub

        Protected Sub saveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            saveData("saved")
            status = "saved"
            savenotificationPanel.Visible = True
            savenotificationPanel2.Visible = True
            '  response.Write("savedbutton_click status = " + status + "<br />")

        End Sub

        Protected Sub toggleViewButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)


        End Sub

        Protected Sub exportPDFButton_Click(ByVal sender As Object, ByVal e As System.EventArgs)


        End Sub

        Protected Sub saveData(ByVal responsestatus As String)

            ' response.Write("saveData line 1156 - status = " + responsestatus + "<br />")

            If examinerType = "unit" Then

                'delete existing unit responses
                Dim myDatabaseUtil5 As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)

                Try
                    myDatabaseUtil5.addParameter("@examinerid", examinerid)
                    myDatabaseUtil5.addParameter("@reportyear", reportyear)
                    myDatabaseUtil5.executeNonQuery("storproc_deleteexaminerunitresponse")
                    myDatabaseUtil5.clearParameters()
                Catch ex As Exception
                    response.Write("ERROR - savedata delete unit response: " & ex.Message())
                End Try

                '___________________________________________________________________________________________________________________________________


                Dim myDataReader As SqlDataReader = Nothing
                Try

                    'get their unit names from the database
                    Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                    myDatabaseUtil.addParameter("@examinerid", examinerid)
                    myDatabaseUtil.addParameter("@completed", "false")
                    myDatabaseUtil.addParameter("@reportyear", reportyear)
                    ' response.Write(" in here storproc_getunits<br />")
                    myDataReader = myDatabaseUtil.getSqlDataReader("storproc_getunits")


                    myDatabaseUtil.clearParameters()

                    If myDataReader.HasRows Then
                        'response.Write("has rows<br />")
                        Dim unitcode As String = ""
                        Dim unitid As String = ""
                        Dim unitname As String = ""
                        Dim unitlevel As String = ""
                        Dim academicsession As String = ""

                        While myDataReader.Read()
                            If Not myDataReader("unitcode") Is DBNull.Value Then
                                unitcode = myDataReader("unitcode")
                            End If

                            If Not myDataReader("unitid") Is DBNull.Value Then
                                unitid = myDataReader("unitid")
                            End If

                            If Not myDataReader("unitname") Is DBNull.Value Then
                                unitname = myDataReader("unitname")
                            End If

                            If Not myDataReader("unitlevel") Is DBNull.Value Then
                                unitlevel = myDataReader("unitlevel")
                            End If

                            If Not myDataReader("academic_session") Is DBNull.Value Then
                                academicsession = myDataReader("academic_session")
                            End If

                            'get their answers from the form 
                            Dim a1_1 As String = ""
                            Dim b1_1 As String = ""
                            Dim c1_1 As String = ""

                            Dim q1yesradio As Radio = FindControl("a1_1_" + unitid)
                            Dim q1noradio As Radio = FindControl("a1_0_" + unitid)

                            If q1yesradio.Checked = True Then
                                a1_1 = "yes"
                            Else
                                a1_1 = "no"
                            End If

                            Dim q2yesradio As Radio = FindControl("b1_1_" + unitid)
                            Dim q2noradio As Radio = FindControl("b1_0_" + unitid)

                            If q2yesradio.Checked = True Then
                                b1_1 = "yes"
                            ElseIf q2noradio.Checked = True Then
                                b1_1 = "no"
                            End If

                            Dim q3yesradio As Radio = FindControl("c1_1_" + unitid)
                            Dim q3noradio As Radio = FindControl("c1_0_" + unitid)

                            If q3yesradio.Checked = True Then
                                c1_1 = "yes"
                            ElseIf q3noradio.Checked = True Then
                                c1_1 = "no"
                            End If
                            '  response.Write(" in here storproc_saveunitquestions<br />")
                            'save the unit question responses
                            Dim myDatabaseUtil2 As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                            myDatabaseUtil2.addParameter("@unitid", unitid)
                            myDatabaseUtil2.addParameter("@examinerid", examinerid)
                            myDatabaseUtil2.addParameter("@a1_1", a1_1)
                            myDatabaseUtil2.addParameter("@b1_1", b1_1)
                            myDatabaseUtil2.addParameter("@c1_1", c1_1)
                            myDatabaseUtil2.addParameter("@reportyear", reportyear)
                            '  response.Write("responsestatus = " + responsestatus + "<br />")
                            myDatabaseUtil2.addParameter("@responsestatus", responsestatus)
                            '  response.Write("in here<br />")
                            'response.Write("unit = " + unitid + "a1_1 = " + a1_1 + "b1_1 = " + b1_1 + "c1_1 = " + c1_1 + "<br />")

                            myDatabaseUtil2.executeNonQuery("storproc_saveunitquestions")
                            myDatabaseUtil2.clearParameters()

                        End While
                    End If

                Catch ex As Exception
                    response.Write("ERROR - savedata saving question responses: " & ex.Message())


                Finally
                    ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
                End Try
            End If
            '___________________________________________________________________________________________________________________________________

            'delete existing examiner responses
            Dim myDatabaseUtil4 As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)

            Try
                myDatabaseUtil4.addParameter("@examinerid", examinerid)
                myDatabaseUtil4.addParameter("@reportyear", reportyear)
                '  response.Write(" in here storproc_deleteexaminerresponse<br />")
                myDatabaseUtil4.executeNonQuery("storproc_deleteexaminerresponse")
                myDatabaseUtil4.clearParameters()
            Catch ex As Exception
                response.Write("ERROR - savedata delete response: " & ex.Message())
            End Try

            '___________________________________________________________________________________________________________________________________

            Try
                'save the rest of the question responses
                Dim myDatabaseUtil3 As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)

                For Each questionid In getQuestionIdArray(examinerType)

                    Dim examinerresponse = ""

                    If request.Form(questionid) IsNot Nothing And request.Form(questionid) <> "" Then

                        'response.Write(" in here storproc_saveresponses<br />")

                        examinerresponse = request.Form(questionid).ToString

                        ' response.Write("question id processed = " + questionid + "<br />")
                        ' response.Write("examiner response = " + examinerresponse + "<br />")

                        myDatabaseUtil3.addParameter("@questionid", questionid)
                        myDatabaseUtil3.addParameter("@examinerid", examinerid)
                        myDatabaseUtil3.addParameter("@response", examinerresponse)
                        myDatabaseUtil3.addParameter("@formtype", examinerType)
                        myDatabaseUtil3.addParameter("@responsestatus", responsestatus)
                        myDatabaseUtil3.addParameter("@reportyear", reportyear)

                        myDatabaseUtil3.executeNonQuery("storproc_saveresponses")
                        myDatabaseUtil3.clearParameters()
                    Else
                        ' response.Write("this questionid is not getting processed " + questionid + "<br />")
                    End If
                Next

            Catch ex As Exception
                response.Write("ERROR - savedata saving question responses: " & ex.Message())

            End Try
            status = responsestatus

            If status = "submitted" And isAdminMode = False Then
                sendConfirmationEmail(examinerid)
            End If
        End Sub

        Public Sub sendConfirmationEmail(ByVal examinerid As String)
            Dim firstname As String = ""
            Dim lastname As String = ""
            Dim email As String = ""
  
            Dim myDataReader As SqlDataReader = Nothing
            Try

                'get their unit names from the database
                Dim myDatabaseUtil As New edc.ljm.common.DatabaseUtil(edc.ljm.common.SiteUtil.DB_EXTERNAL_EXAMINERS_FEEDBACK)
                myDatabaseUtil.addParameter("@examinerid", examinerid)
    
                myDataReader = myDatabaseUtil.getSqlDataReader("storproc_getexaminer")

                myDatabaseUtil.clearParameters()

                If myDataReader.HasRows Then
                    'response.Write("has rows<br />")
            

                    While myDataReader.Read()
                        If Not myDataReader("first_name") Is DBNull.Value Then
                            firstname = myDataReader("first_name")
                        End If

                        If Not myDataReader("last_name") Is DBNull.Value Then
                            lastname = myDataReader("last_name")
                        End If

                        If Not myDataReader("email") Is DBNull.Value Then
                            email = myDataReader("email")
                        End If

                    End While
                End If

            Catch ex As Exception
                response.Write("ERROR - sendConfirmationEmail1: " & ex.Message())
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try

            Try
                Dim client As New SmtpClient("server-mail.solent.ac.uk")
                Dim toAddress As New MailAddress("portal@solent.ac.uk", "Learning Technologies Web Team")
                ' Dim toAddress As New MailAddress("asqs.externalexaminers@solent.ac.uk")
                Dim fromAddress As New MailAddress("asqs.externalexaminers@solent.ac.uk", "Southampton Solent University")
                Dim message As New MailMessage(fromAddress, toAddress)
                message.CC.Add("helen.sharma@solent.ac.uk")
                Dim formurl As String = ""
                Dim body As String = ""

                message.IsBodyHtml = True
                message.Subject = "External Examiners Annual Report " + reportyear + " - Thank you for your feedback"

                Dim claimformlink As String = "http://portal.solent.ac.uk/documents/academic-services/external-examiners-claim-form-sal06.pdf"

                Dim pdfdownloadlink As String = "http://portal.solent.ac.uk/portal-apps/external-examiners/feedback/form.aspx?id=" + examinerid + "&type=" + examinerType + "&print=true"


                body = body + "<p><strong>External Examiners Annual Report " + reportyear + " </strong></p>" + vbCrLf
                body = body + "<p>Thank you for completing the Southampton Solent University external examiners annual report.</p>" + vbCrLf
                body = body + "<p>Any minor spelling mistakes will be corrected before being circulated to relevant University staff. The University will aim to respond to your report within six weeks.</p>" + vbCrLf
                body = body + "<p>You can download a copy of your responses by using the 'Download form as PDF' link at the bottom of the feedback form, or by using this link:<br />" + vbCrLf
                body = body + "<a href=""" + pdfdownloadlink + """>" + pdfdownloadlink + "</a></p>" + vbCrLf
                body = body + "<p>If redirected to the portal log in screen please use the login details provided in the original email. </p>" + vbCrLf
                body = body + "<p>You are now able to download the external examiner claim form from: <a href=""" + claimformlink + """>" + claimformlink + "</a></p>" + vbCrLf
                body = body + "<p>Please return your claim form to Quality Officer, Academic Services, Southampton Solent University, East Park Terrace, Southampton, SO14 0YN.</p>"
                body = body + "<p>Thank you for your assistance in completing this report.</p>" + vbCrLf
                body = body + "<p>Kind regards</p>" + vbCrLf
                body = body + "<p>Andrew Stevenson<br />Quality Officer, Academic Services" + vbCrLf
                body = body + "<br />Southampton Solent University<br /> " + vbCrLf
                body = body + "<br />Tel: +44 (0)23 8201 3978" + vbCrLf
                body = body + "<br />Email: <a href=""mailto:as.externalexaminers@solent.ac.uk"">as.externalexaminers@solent.ac.uk</a></p>" + vbCrLf
                message.Body = body
                client.Send(message)

            Catch ex As Exception
                'response.Write("ERROR - sendConfirmationEmail2: " + ex.Message + " - " + ex.InnerException.ToString)
            End Try
        End Sub
    End Class
End Namespace

