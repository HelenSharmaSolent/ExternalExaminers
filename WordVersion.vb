Imports System.Web
Imports CMS_API.WebUI.WebControls
Imports System.Data.SqlClient
Imports System.Web.UI.WebControls
Imports System.Web.UI.ClientScriptManager
Imports System.Web.SessionState
Imports edc.hs.portal2.ExternalExaminers
Imports System.Net.Mail
Imports System.Text
Imports System.Web.UI
Imports System.IO


Namespace ExternalExaminers
    Public Class WordVersion

        Inherits BasePlaceHolderControl

        Dim reportyear As String = ""
        Dim examinerid As String
        Dim examinerType As String
        Dim questionid As String = ""
        Dim outputDiv As New Div

        Private request As HttpRequest
        Private response As HttpResponse
        Private session As HttpSessionState

        Public Sub addToOutput(ByVal controlIn As Object)
            Me.Controls.Add(controlIn)
        End Sub

        Private Sub ExternalExaminersFormUnit_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init


        End Sub
        Private Sub WordVersion_Load(sender As Object, e As EventArgs) Handles Me.Load
           
            dostuff()

        End Sub
        Public Sub dostuff()

            request = HttpContext.Current.Request
            response = HttpContext.Current.Response

            response.Clear()
            response.ClearContent()
            response.ClearHeaders()
            response.Write("<head><meta http-equiv=Content-Type content=:" & """"c & "text/html; charset=utf-8" & """"c & "></head>")
            response.ContentType = "application/ms-word"
            response.AddHeader("content-disposition", "inline; filename=test.doc")


            outputDiv.ID = "external-examiners-feedback-application"
            addToOutput(outputDiv)

            createwordversion()

            Dim sb As New StringBuilder
            Dim tw As New HtmlTextWriter(New StringWriter(sb))

            outputDiv.RenderControl(tw)

            response.Write(sb.ToString)
            response.End()

        End Sub

        Public Sub createwordversion()


            Dim eeForm As New Form
            Dim eeRecord As New Record

            If String.IsNullOrEmpty(request.QueryString("id")) Or String.IsNullOrEmpty(request.QueryString("type")) Then

                addToOutput(New P("Examiner ID not supplied. Please try the supplied link again or contact as.externalexaminers@solent.ac.uk"))

            Else 'run application

                Try
                    reportyear = eeForm.getreportdate()
                    examinerid = request.QueryString("id")
                    examinerType = request.QueryString("type")

                Catch ex As Exception
                    response.Write("ERROR: ExternalExaminersFormUnit_Init 1 " + ex.ToString)
                End Try

            End If

            Try
                Dim examinertypename As String = ""

                If examinerType = "unit" Then
                    examinertypename = "Unit"
                ElseIf examinerType = "award" Then
                    examinertypename = "Progression and award"
                End If


                outputDiv.Controls.Add(New Heading(2, examinertypename + " external examiner " + reportyear))
                outputDiv.Controls.Add(New P(examinertypename + " external examiner reports will be received and considered by Academic Services. They will also be considered formally as part of the course review process. You will normally receive a response to your report within six weeks of submission. A summary of themes emerging from external examiner reports is presented to the University’s Academic Board."))
                outputDiv.Controls.Add(New P("External examiner reports are made available in full to students. They also constitute recorded information held by the University and are therefore open to disclosure if requested under the Freedom of Information Act. "))
                outputDiv.Controls.Add(New P("The University will take very seriously any negative answers and will work with the course teams concerned to address the issues raised in a supportive and developmental way."))
                outputDiv.Controls.Add(New P("<strong>Please note: Individual students or members of staff should not be identified in the report.</strong>"))
                outputDiv.Controls.Add(New P("You will be prompted to use free text boxes within your report in order to provide qualitative feedback. This will be used to disseminate good practice and identify areas for enhancement. "))
                outputDiv.Controls.Add(New Heading(2, "General information"))
                outputDiv.Controls.Add(New P("This information is held in our system. If any information is incorrect then please contact as.externalexaminers@solent.ac.uk"))
                outputDiv.Controls.Add(eeRecord.createRecord(examinerid, False, "word"))
            Catch ex As Exception
                response.Write("ERROR: ExternalExaminersFormUnit_Init 2 " + ex.ToString)
            End Try


            Try
                'create unit questions
                '---------------------------------------------------------------------------------------------
                If examinerType = "unit" Then

                    outputDiv.Controls.Add(New Heading(2, "Section 1 - Standards"))

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

                    outputDiv.Controls.Add(rowdiv)

                    createUnitQuestions(examinerid)

                End If
            Catch ex As Exception
                response.Write("ERROR: ExternalExaminersFormUnit_Init 3 " + ex.ToString)
            End Try

            Try
                createQuestionsAndAnswers(examinerid, examinerType, 999)
            Catch ex As Exception
                response.Write("ERROR: ExternalExaminersFormUnit_Init 4 " + ex.ToString)
            End Try



        End Sub

        Private Sub createQuestionsAndAnswers(ByVal examinerid As String, ByVal examinertype As String, ByVal panelIn As String)

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


                        If display = True Then

                            Dim numberDiv As New Div()

                            If number.Length > 0 Then
                                numberDiv.CssClass = "number"
                                numberDiv.InnerHTML = "<strong>" + number + "</strong>"
                            End If

                            Dim questiontextDiv As New Div()
                            questiontextDiv.CssClass = "questiontext"
                            questiontextDiv.InnerHTML = "<strong>" + text + "</strong>"

                            Dim textanswer As New Div()
                            textanswer.CssClass = "wordanswertext"
                            textanswer.InnerHTML = questionresponse

                            Dim radioanswer As New Div()
                            radioanswer.CssClass = "wordanswertextradiocomment"
                            Dim radioanswertext As String = ""


                            If radiocomment = True Then 'normal comment
                                Dim eeform As New Form
                                radioanswertext = eeform.getRadioCommentWord(questionid, questionresponse)
                                'radioanswer.InnerHTML = "hello i am a radio comment"
                            End If
                            radioanswer.Controls.Add(New P(radioanswertext))

                            rowDiv.Controls.Add(numberDiv)
                            rowDiv.Controls.Add(questiontextDiv)
                            rowDiv.Controls.Add(New BR)
                            rowDiv.Controls.Add(textanswer)
                            rowDiv.Controls.Add(radioanswer)
                            rowDiv.Controls.Add(New BR)
                            rowDiv.Controls.Add(New BR)


                            Dim radiobuttonDiv As New Div
                            radiobuttonDiv.CssClass = "radiobuttoncontainer"


                            outputDiv.Controls.Add(rowDiv)
                        End If

                    End While
                Else

                End If
            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers CreateQuestionsandAnswers WordVersion: " & ex.Message())
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader)
            End Try

        End Sub

        Private Function getQuestionIdArray(ByVal examinertype As String)

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

        Private Sub createUnitQuestions(ByVal examineridIn As String)

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

                    'Dim tableheader As New TableHeaderRow

                    'Dim tableheadercell1 As New TableHeaderCell
                    'Dim tableheadercell2 As New TableHeaderCell
                    'Dim tableheadercell3 As New TableHeaderCell
                    'Dim tableheadercell4 As New TableHeaderCell

                    'tableheadercell2.Text = ""
                    'tableheadercell2.ColumnSpan = 2
                    'tableheadercell3.Text = ""
                    'tableheadercell3.ColumnSpan = 2
                    'tableheadercell4.Text = ""
                    'tableheadercell4.ColumnSpan = 2

                    'tableheader.Controls.Add(tableheadercell1)
                    'tableheader.Controls.Add(tableheadercell2)
                    'tableheader.Controls.Add(tableheadercell3)
                    'tableheader.Controls.Add(tableheadercell4)
                    'unittable.Controls.Add(tableheader)

                    Dim tablerowsecond As New TableRow

                    Dim tablecellsecond1 As New TableHeaderCell
                    Dim tablecellsecond2 As New TableHeaderCell
                    ' Dim tablecellsecond3 As New TableHeaderCell
                    Dim tablecellsecond4 As New TableHeaderCell
                    ' Dim tablecellsecond5 As New TableHeaderCell
                    Dim tablecellsecond6 As New TableHeaderCell
                    ' Dim tablecellsecond7 As New TableHeaderCell

                    tablecellsecond1.Text = "Unit information"
                    tablecellsecond2.Text = "Were the standards set for the units appropriate for their level?"
                    ' tablecellsecond3.Text = "No"
                    tablecellsecond4.Text = "Were the standards of student performance comparable with similar programmes or subjects in other UK institutions with which you are familiar?"
                    '  tablecellsecond5.Text = "No"
                    tablecellsecond6.Text = "Were you sent copies of all assessment tasks prior to their release to students?"
                    ' tablecellsecond7.Text = "No"

                    tablerowsecond.Controls.Add(tablecellsecond1)
                    tablerowsecond.Controls.Add(tablecellsecond2)
                    ' tablerowsecond.Controls.Add(tablecellsecond3)
                    tablerowsecond.Controls.Add(tablecellsecond4)
                    ' tablerowsecond.Controls.Add(tablecellsecond5)
                    tablerowsecond.Controls.Add(tablecellsecond6)
                    'tablerowsecond.Controls.Add(tablecellsecond7)

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
                        ' Dim tablecell3 As New TableCell
                        Dim tablecell4 As New TableCell
                        '  Dim tablecell5 As New TableCell
                        Dim tablecell6 As New TableCell
                        ' Dim tablecell7 As New TableCell

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


                        tablecell1.Controls.Add(New Span(unitname + " (" + unitcode + ") " + unitlevel + " - " + academicsession))

                        'tablecell2.Controls.Add(radiobutton1)
                        'tablecell3.Controls.Add(radiobutton2)
                        'tablecell4.Controls.Add(radiobutton3)
                        'tablecell5.Controls.Add(radiobutton4)
                        'tablecell6.Controls.Add(radiobutton5)
                        'tablecell7.Controls.Add(radiobutton6)

                        tablecell2.Controls.Add(New Span(a1_1_response))
                        ' tablecell3.Controls.Add(New Span(""))
                        tablecell4.Controls.Add(New Span(b1_1_response))
                        ' tablecell5.Controls.Add(New Span(""))
                        tablecell6.Controls.Add(New Span(c1_1_response))
                        'tablecell7.Controls.Add(New Span(""))

                        tablerow.Controls.Add(tablecell1)
                        tablerow.Controls.Add(tablecell2)
                        ' tablerow.Controls.Add(tablecell3)
                        tablerow.Controls.Add(tablecell4)
                        ' tablerow.Controls.Add(tablecell5)
                        tablerow.Controls.Add(tablecell6)
                        ' tablerow.Controls.Add(tablecell7)

                        unittable.Controls.Add(tablerow)



                        iRow = iRow + 1


                    End While

                    outputDiv.Controls.Add(unittable)
                Else
                    '  response.Write("no rows<br />")
                End If

            Catch ex As Exception
                response.Write("ERROR - ExternalExaminers CreateUnitQuestions2: " & ex.Message())
            Finally
                ljm.common.DatabaseUtil.closeSqlDataReader(myDataReader2)

            End Try


        End Sub




    End Class
End Namespace