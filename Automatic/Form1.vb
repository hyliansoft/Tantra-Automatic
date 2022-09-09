
Option Explicit On

Imports System.Runtime.InteropServices
Imports System.IO

'net472
'net6.0 - windows

Public Class Form1

#Region "Cabecera"

    Dim processlist As Process() = Process.GetProcesses()
    Dim myProcess As String = "HTLauncher"
    Dim seleccionado As String
    Dim seleccionadoID As Integer
    Dim proceso As Process
    Dim activo As Boolean = False

    Dim coorHPX As Integer = 0
    Dim coorHPY As Integer = 0
    Dim coorManaX As Integer = 0
    Dim coorManaY As Integer = 0


    Dim ruta As String = "presets\"


    Dim handle As IntPtr

    Public Const KEY_E = &H45
    Public Const KEY_R = &H52
    Public Const KEY_F = &H46
    Public Const KEY_W = &H57

    Public Const KEY_F1 = &H70
    Public Const KEY_F2 = &H71
    Public Const KEY_F3 = &H72
    Public Const KEY_F4 = &H73
    Public Const KEY_F5 = &H74
    Public Const KEY_F6 = &H75
    Public Const KEY_F7 = &H76
    Public Const KEY_F8 = &H77
    Public Const KEY_F9 = &H78
    Public Const KEY_F10 = &H79

    Public Const KEY_0 = &H30
    Public Const KEY_1 = &H31
    Public Const KEY_2 = &H32
    Public Const KEY_3 = &H33
    Public Const KEY_4 = &H34
    Public Const KEY_5 = &H35
    Public Const KEY_6 = &H36
    Public Const KEY_7 = &H37
    Public Const KEY_8 = &H38
    Public Const KEY_9 = &H39


    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function


    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function PostMessage(ByVal hWnd As IntPtr, ByVal Msg As UInteger, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As Boolean
    End Function

    Private Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Long) As Integer

    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)>
    Private Shared Function SetWindowText(ByVal hwnd As IntPtr, ByVal lpString As String) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Public Shared Function GetWindowDC(ByVal hwnd As IntPtr) As IntPtr
        'Do not try to name this method "GetDC" it will say that user32 doesnt have GetDC !!!
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Public Shared Function ReleaseDC(ByVal hwnd As IntPtr, ByVal hdc As IntPtr) As Int32

    End Function

    <DllImport("gdi32.dll", SetLastError:=True)>
    Public Shared Function GetPixel(ByVal hdc As IntPtr, ByVal nXPos As Integer, ByVal nYPos As Integer) As UInteger

    End Function

    <System.Runtime.InteropServices.DllImport("user32.dll")>
    Private Shared Function FindWindow(
    ByVal lpClassName As String,
    ByVal lpWindowName As String) As System.IntPtr
    End Function

#End Region

#Region "Procesos"
    Public Sub cargarprocesos()

        ListBox1.Items.Clear()
        processlist = Process.GetProcesses()

        For Each proc As Process In processlist
            If Not String.IsNullOrEmpty(proc.MainWindowTitle) And proc.ProcessName = myProcess Then
                ListBox1.Items.Add(proc.MainWindowTitle.ToString())
            End If
        Next
    End Sub

    Private Sub LoadPrincipal(sender As Object, e As EventArgs) Handles MyBase.Load

        cargarprocesos()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        cargarprocesos()
    End Sub


    Private Sub CambiarNombreProceso(sender As Object, e As EventArgs) Handles Button2.Click

        If Not String.IsNullOrEmpty(seleccionado) Then
            If Not String.IsNullOrEmpty(TextBox1.Text) Then
                If (TextBox1.Text.Length <= 20) Then
                    For Each proc As Process In processlist
                        If Not String.IsNullOrEmpty(proc.MainWindowTitle) And proc.MainWindowTitle.ToString = seleccionado And proc.Id = seleccionadoID Then
                            SetWindowText(proc.MainWindowHandle, TextBox1.Text)
                            cargarprocesos()
                        End If
                    Next
                Else
                    MsgBox("Ingrese un nombre mas corto")
                End If
            Else
                MsgBox("Ingrese un nombre válido")

            End If
        Else
            MsgBox("Seleccione un proceso de la lista")
        End If

    End Sub

    Private Sub seleccionarProceso(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If (ListBox1.SelectedIndex >= 0) Then
            seleccionado = ListBox1.GetItemText(ListBox1.SelectedItem)

            TextBox1.Text = seleccionado
            TextBox1.Select()

            For Each proc As Process In processlist
                If Not String.IsNullOrEmpty(proc.MainWindowTitle) And proc.MainWindowTitle.ToString = seleccionado Then
                    proceso = proc
                    seleccionadoID = proceso.Id
                    Try
                        handle = FindWindow(vbNullString, seleccionado)
                    Catch ex As Exception

                    End Try

                End If
            Next

        End If
    End Sub

#End Region

#Region "AutoPots"

    Public Function GetPixelColor(ByVal x As Integer, ByVal y As Integer) As Color
        Dim hdc As IntPtr = GetWindowDC(IntPtr.Zero)
        Dim pixel As UInteger = GetPixel(hdc, x, y)
        Dim color As Color
        ReleaseDC(IntPtr.Zero, hdc)
        color = Color.FromArgb(Int(pixel And &HFF),
        Int(pixel And &HFF00) >> 8,
        Int(pixel And &HFF0000) >> 16)
        Return color
    End Function

    Private Sub selectHP(sender As Object, e As EventArgs) Handles ButtonHP.Click


        If Not String.IsNullOrEmpty(seleccionado) Then
            TimerHP.Start()
            TimerHP.Interval = 100
            Me.Enabled = False
            panelpotas.Visible = True
            panelpotas.BringToFront()
            Try
                AppActivate(proceso.Id)
            Catch ex As Exception
                MsgBox("No se encuentra el proceso", MsgBoxStyle.Information, "Proceso")
            End Try

        Else
            MsgBox("Debe seleccionar un proceso", MsgBoxStyle.Information, "Error")
        End If


    End Sub

    Private Sub timer_HP(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerHP.Tick

        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            coorHPX = MousePosition.X
            coorHPY = MousePosition.Y
            Label24.Text = "X=" & MousePosition.X.ToString & "  Y=" & MousePosition.Y.ToString
            Label24.ForeColor = Color.Green
            Me.Enabled = True
            TimerHP.Stop()
            panelpotas.Visible = False
            panelpotas.SendToBack()
            Me.Activate()

        End If

    End Sub

    Private Sub timer_MANA(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerMana.Tick

        If MouseButtons = Windows.Forms.MouseButtons.Right Then
            coorManaX = MousePosition.X
            coorManaY = MousePosition.Y
            Label25.Text = "X=" & MousePosition.X.ToString & "  Y=" & MousePosition.Y.ToString
            Label25.ForeColor = Color.Green
            Me.Enabled = True
            TimerMana.Stop()
            panelpotas.Visible = False
            panelpotas.SendToBack()
            Me.Activate()
        End If

    End Sub


    Private Sub selectMANA(sender As Object, e As EventArgs) Handles Buttonmana.Click

        If Not String.IsNullOrEmpty(seleccionado) Then
            TimerMANA.Start()
            TimerMANA.Interval = 100
            Me.Enabled = False
            panelpotas.Visible = True
            panelpotas.BringToFront()
            Try
                AppActivate(proceso.Id)
            Catch ex As Exception
                MsgBox("No se encuentra el proceso", MsgBoxStyle.Information, "Proceso")
            End Try

        Else
            MsgBox("Debe seleccionar un proceso", MsgBoxStyle.Information, "Error")
        End If

    End Sub

    Private Sub TimerPotas_Tick(sender As Object, e As EventArgs) Handles TimerPotas.Tick

        Dim colorHP, colorMana As Color

        colorHP = GetPixelColor(coorHPX, coorHPY)
        colorMana = GetPixelColor(coorManaX, coorManaY)

        Me.Refresh()

        'Check HP 

        If colorHP <> Color.Empty And coorHPX > 0 And coorHPY > 0 Then
            If colorHP.A = 255 And colorHP.R = 0 And colorHP.G = 0 And colorHP.B = 0 Then
                SendMessage(handle, &H100, KEY_F9, 0)
            End If
        End If

        'Check MANA 

        If colorMana <> Color.Empty And coorHPX > 0 And coorHPY > 0 Then
            If colorMana.A = 255 And colorMana.R = 0 And colorMana.G = 0 And colorMana.B = 0 Then
                SendMessage(handle, &H100, KEY_F10, 0)
            End If
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        MsgBox("Al hacer clic en el boton 'Selec nivel HP' se redirigira a Tantra. Usted deberá seleccionar un lugar en la barra donde desea que se consuma una pocion. La seleccion se realiza haciendo clic DERECHO.  Se recomienda seleccionar dentro de la barra no hayan numeros.    No mueva la ventana de HP una vez seleccionado.", MsgBoxStyle.Information, "Ayuda")

    End Sub

    Private Sub CheckBoxautopots_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxautopots.CheckedChanged

        If (CheckBoxautopots.Checked) Then
            skillf9.Enabled = False
            skillf10.Enabled = False
            skillf9.Text = ""
            skillf10.Text = ""
            checkf9.Enabled = False
            checkf10.Enabled = False
            checkf9.Checked = False
            checkf10.Checked = False
            Label13.Enabled = False
            Label14.Enabled = False
        Else
            skillf9.Enabled = True
            skillf10.Enabled = True
            skillf9.Text = "1"
            skillf10.Text = "1"
            checkf9.Enabled = True
            checkf10.Enabled = True
            checkf9.Checked = False
            checkf10.Checked = False
            Label13.Enabled = True
            Label14.Enabled = True
        End If

    End Sub

#End Region

#Region "Keypress"



    Private Sub startclick(sender As Object, e As EventArgs) Handles start.Click

        cargarprocesos()

        If (CheckBoxautopots.Checked And coorHPX = 0 And coorHPY = 0) Then

            MsgBox("Si marca la opcion autopost debe elegir las coordenadas en su barra de HP/Mana.", MsgBoxStyle.Information, "AutoPot")

            Exit Sub
        End If

        If Not String.IsNullOrEmpty(seleccionado) Then
            Try
                AppActivate(proceso.Id)
            Catch ex As Exception
                MsgBox("No se encuentra el proceso", MsgBoxStyle.Information, "Proceso")
            End Try


            If activo = True Then
                activo = False
                start.Text = "Start"

                GroupBoxautopot.Enabled = True
                Timerprincipal.Stop()
                TimerE.Stop()
                TimerR.Stop()
                Timerrecojer.Stop()

                TimerSkill1.Stop()
                Timerskill2.Stop()
                TimerSkill3.Stop()
                Timerskill4.Stop()
                TimerSkill5.Stop()
                Timerskill6.Stop()
                TimerSkill7.Stop()
                Timerskill8.Stop()
                TimerSkill9.Stop()
                Timerskill0.Stop()

                TimerSkillF1.Stop()
                TimerskillF2.Stop()
                TimerSkillF3.Stop()
                TimerskillF4.Stop()
                TimerSkillF5.Stop()
                TimerskillF6.Stop()
                TimerSkillF7.Stop()
                TimerskillF8.Stop()
                TimerSkillF9.Stop()
                TimerskillF10.Stop()
                TimerPotas.Stop()
                timerstuck.Stop()
            Else

                timerstuck.Start()
                GroupBoxautopot.Enabled = False
                activo = True
                start.Text = "Stop"
                Timerprincipal.Interval = 1000
                Timerprincipal.Start()

                If (CheckBoxautopots.Checked) Then
                    TimerPotas.Interval = 1000
                    TimerPotas.Start()
                End If

            End If

        Else
            MsgBox("Seleccione un proceso de la lista", MsgBoxStyle.Information, "Proceso")
        End If


    End Sub

    'SOLO NUMEROS
    Private Sub textbox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles skill1.KeyPress, skill2.KeyPress, skill3.KeyPress, skill4.KeyPress, skill5.KeyPress, skill6.KeyPress, skill7.KeyPress, skill8.KeyPress, skill9.KeyPress, skill0.KeyPress, skillf1.KeyPress, skillf2.KeyPress, skillf3.KeyPress, skillf4.KeyPress, skillf5.KeyPress, skillf6.KeyPress, skillf7.KeyPress, skillf8.KeyPress, skillf9.KeyPress, skillf10.KeyPress, skille.KeyPress, skillr.KeyPress, TextBoxrecojer.KeyPress
        If Asc(e.KeyChar) <> 8 Then
            If Asc(e.KeyChar) < 48 Or Asc(e.KeyChar) > 57 Then
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub TimerE_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerE.Tick
        If (skille.Text <> String.Empty) Then
            If (CInt(skille.Text) > 0) Then
                TimerE.Interval = CInt(skille.Text) * 1000
            End If
        End If
        Threading.Thread.Sleep(500) 'ms
        SendMessage(handle, &H100, KEY_E, 0)

    End Sub

    Private Sub TimerStuck_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerstuck.Tick

        TimerE.Interval = 1000
        antistuck()

    End Sub

    Private Sub antistuck()
        If checkstuck.Checked Then
            SendMessage(handle, &H100, KEY_W, 0)
        End If
    End Sub


    Private Sub TimerR_Tick(sender As Object, e As EventArgs) Handles TimerR.Tick
        If (skillr.Text <> String.Empty) Then
            If (CInt(skillr.Text) > 0) Then
                TimerR.Interval = CInt(skillr.Text) * 1000
            End If
        End If

        SendMessage(handle, &H100, KEY_R, 0)
    End Sub

    'recojer

    Private Sub Timerrecojer_Tick(sender As Object, e As EventArgs) Handles Timerrecojer.Tick
        If (TextBoxrecojer.Text <> String.Empty) Then
            If (CInt(TextBoxrecojer.Text) > 0) Then
                Timerrecojer.Interval = CInt(TextBoxrecojer.Text) * 1000
            End If
        End If
        SendMessage(handle, &H100, KEY_F, 0)
    End Sub

    Private Sub Timerprincipal_Tick(sender As Object, e As EventArgs) Handles Timerprincipal.Tick

        '' ATAQUE BASICO
        If (checkbasico.Checked) Then
            If (skille.Text <> String.Empty) Then
                If (CInt(skille.Text) > 0) Then
                    If (TimerE.Enabled = False) Then
                        TimerE.Start()
                        Threading.Thread.Sleep(500) 'ms
                    End If
                End If
            Else
                TimerE.Stop()
            End If

            If (skillr.Text <> String.Empty) Then
                If (CInt(skillr.Text) > 0) Then
                    If (TimerR.Enabled = False) Then
                        TimerR.Start()
                    End If
                Else
                    TimerR.Stop()
                End If
            Else
                TimerR.Stop()
            End If

        Else
            TimerE.Stop()
            TimerR.Stop()
        End If

        If (CheckBoxrecojer.Checked And TextBoxrecojer.Text <> String.Empty) Then
            If (CInt(TextBoxrecojer.Text) >= 1) Then
                Timerrecojer.Start()
                Timerrecojer.Interval = CInt(TextBoxrecojer.Text) * 1000
            Else
                Timerrecojer.Stop()
            End If
        Else
            Timerrecojer.Stop()
        End If

        'SKILLS 

        If (check1.Checked And skill1.Text <> String.Empty) Then
            If (CInt(skill1.Text) >= 1) Then
                TimerSkill1.Start()
                TimerSkill1.Interval = CInt(skill1.Text) * 1000
            Else
                TimerSkill1.Stop()
            End If
        Else
            TimerSkill1.Stop()
        End If

        If (check2.Checked And skill2.Text <> String.Empty) Then
            If (CInt(skill2.Text) >= 1) Then
                Timerskill2.Start()
                Timerskill2.Interval = CInt(skill2.Text) * 1000
            Else
                Timerskill2.Stop()
            End If
        Else
            Timerskill2.Stop()
        End If


        If (check3.Checked And skill3.Text <> String.Empty) Then
            If (CInt(skill3.Text) >= 1) Then
                TimerSkill3.Start()
                TimerSkill3.Interval = CInt(skill3.Text) * 1000
            Else
                TimerSkill3.Stop()
            End If
        Else
            TimerSkill3.Stop()
        End If


        If (check4.Checked And skill4.Text <> String.Empty) Then
            If (CInt(skill4.Text) >= 1) Then
                Timerskill4.Start()
                Timerskill4.Interval = CInt(skill4.Text) * 1000
            Else
                Timerskill4.Stop()
            End If
        Else
            Timerskill4.Stop()
        End If


        If (check5.Checked And skill5.Text <> String.Empty) Then
            If (CInt(skill5.Text) >= 1) Then
                TimerSkill5.Start()
                TimerSkill5.Interval = CInt(skill5.Text) * 1000
            Else
                TimerSkill5.Stop()
            End If
        Else
            TimerSkill5.Stop()
        End If


        If (check6.Checked And skill6.Text <> String.Empty) Then
            If (CInt(skill6.Text) >= 1) Then
                Timerskill6.Start()
                Timerskill6.Interval = CInt(skill6.Text) * 1000
            Else
                Timerskill6.Stop()
            End If
        Else
            Timerskill6.Stop()
        End If


        If (check7.Checked And skill7.Text <> String.Empty) Then
            If (CInt(skill7.Text) >= 1) Then
                TimerSkill7.Start()
                TimerSkill7.Interval = CInt(skill7.Text) * 1000
            Else
                TimerSkill7.Stop()
            End If
        Else
            TimerSkill7.Stop()
        End If


        If (check8.Checked And skill8.Text <> String.Empty) Then
            If (CInt(skill8.Text) >= 1) Then
                Timerskill8.Start()
                Timerskill8.Interval = CInt(skill8.Text) * 1000
            Else
                Timerskill8.Stop()
            End If
        Else
            Timerskill8.Stop()
        End If


        If (check9.Checked And skill9.Text <> String.Empty) Then
            If (CInt(skill9.Text) >= 1) Then
                TimerSkill9.Start()
                TimerSkill9.Interval = CInt(skill9.Text) * 1000
            Else
                TimerSkill9.Stop()
            End If
        Else
            TimerSkill9.Stop()
        End If


        If (check0.Checked And skill0.Text <> String.Empty) Then
            If (CInt(skill0.Text) >= 1) Then
                Timerskill0.Start()
                Timerskill0.Interval = CInt(skill0.Text) * 1000
            Else
                Timerskill0.Stop()
            End If
        Else
            Timerskill0.Stop()
        End If




        'SKILLS F1 - F10

        If (checkf1.Checked And skillf1.Text <> String.Empty) Then
            If (CInt(skillf1.Text) >= 1) Then
                TimerSkillF1.Start()
                TimerSkillF1.Interval = CInt(skillf1.Text) * 1000
            Else
                TimerSkillF1.Stop()
            End If
        Else
            TimerSkillF1.Stop()
        End If

        If (checkf2.Checked And skillf2.Text <> String.Empty) Then
            If (CInt(skillf2.Text) >= 1) Then
                TimerskillF2.Start()
                TimerskillF2.Interval = CInt(skillf2.Text) * 1000
            Else
                TimerskillF2.Stop()
            End If
        Else
            TimerskillF2.Stop()
        End If

        If (checkf3.Checked And skillf3.Text <> String.Empty) Then
            If (CInt(skillf3.Text) >= 1) Then
                TimerSkillF3.Start()
                TimerSkillF3.Interval = CInt(skillf3.Text) * 1000
            Else
                TimerSkillF3.Stop()
            End If
        Else
            TimerSkillF3.Stop()
        End If

        If (checkf4.Checked And skillf4.Text <> String.Empty) Then
            If (CInt(skillf4.Text) >= 1) Then
                TimerskillF4.Start()
                TimerskillF4.Interval = CInt(skillf4.Text) * 1000
            Else
                TimerskillF4.Stop()
            End If
        Else
            TimerskillF4.Stop()
        End If

        If (checkf5.Checked And skillf5.Text <> String.Empty) Then
            If (CInt(skillf5.Text) >= 1) Then
                TimerSkillF5.Start()
                TimerSkillF5.Interval = CInt(skillf5.Text) * 1000
            Else
                TimerSkillF5.Stop()
            End If
        Else
            TimerSkillF5.Stop()
        End If

        If (checkf6.Checked And skillf6.Text <> String.Empty) Then
            If (CInt(skillf6.Text) >= 1) Then
                TimerskillF6.Start()
                TimerskillF6.Interval = CInt(skillf6.Text) * 1000
            Else
                TimerskillF6.Stop()
            End If
        Else
            TimerskillF6.Stop()
        End If

        If (checkf7.Checked And skillf7.Text <> String.Empty) Then
            If (CInt(skillf7.Text) >= 1) Then
                TimerSkillF7.Start()
                TimerSkillF7.Interval = CInt(skillf7.Text) * 1000
            Else
                TimerSkillF7.Stop()
            End If
        Else
            TimerSkillF7.Stop()
        End If

        If (checkf8.Checked And skillf8.Text <> String.Empty) Then
            If (CInt(skillf8.Text) >= 1) Then
                TimerskillF8.Start()
                TimerskillF8.Interval = CInt(skillf8.Text) * 1000
            Else
                TimerskillF8.Stop()
            End If
        Else
            TimerskillF8.Stop()
        End If



        If (checkf9.Checked And skillf9.Text <> String.Empty) Then
            If (CInt(skillf9.Text) >= 1) Then
                TimerSkillF9.Start()
                TimerSkillF9.Interval = CInt(skillf9.Text) * 1000
            Else
                TimerSkillF9.Stop()
            End If
        Else
            TimerSkillF9.Stop()
        End If

        If (checkf10.Checked And skillf10.Text <> String.Empty) Then
            If (CInt(skillf10.Text) >= 1) Then
                TimerskillF10.Start()
                TimerskillF10.Interval = CInt(skillf10.Text) * 1000
            Else
                TimerskillF10.Stop()
            End If
        Else
            TimerskillF10.Stop()
        End If


    End Sub
    Private Sub TimerSkill1Tick(sender As Object, e As EventArgs) Handles TimerSkill1.Tick
        If (check1.Checked And skill1.Text <> String.Empty) Then
            If (CInt(skill1.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_1, 0)
            Else
                TimerSkill1.Stop()
            End If
        Else
            TimerSkill1.Stop()
        End If

    End Sub
    Private Sub TimerSkill2Tick(sender As Object, e As EventArgs) Handles Timerskill2.Tick
        If (check2.Checked And skill2.Text <> String.Empty) Then
            If (CInt(skill2.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_2, 0)
            Else
                Timerskill2.Stop()
            End If
        Else
            Timerskill2.Stop()
        End If

    End Sub
    Private Sub TimerSkill3Tick(sender As Object, e As EventArgs) Handles TimerSkill3.Tick
        If (check3.Checked And skill3.Text <> String.Empty) Then
            If (CInt(skill3.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_3, 0)
            Else
                TimerSkill3.Stop()
            End If
        Else
            TimerSkill3.Stop()
        End If

    End Sub
    Private Sub TimerSkill4Tick(sender As Object, e As EventArgs) Handles Timerskill4.Tick
        If (check4.Checked And skill4.Text <> String.Empty) Then
            If (CInt(skill4.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_4, 0)
            Else
                Timerskill4.Stop()
            End If
        Else
            Timerskill4.Stop()
        End If

    End Sub
    Private Sub TimerSkill5Tick(sender As Object, e As EventArgs) Handles TimerSkill5.Tick
        If (check5.Checked And skill5.Text <> String.Empty) Then
            If (CInt(skill5.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_5, 0)
            Else
                TimerSkill5.Stop()
            End If
        Else
            TimerSkill5.Stop()
        End If

    End Sub
    Private Sub TimerSkill6Tick(sender As Object, e As EventArgs) Handles Timerskill6.Tick
        If (check6.Checked And skill6.Text <> String.Empty) Then
            If (CInt(skill6.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_6, 0)
            Else
                Timerskill6.Stop()
            End If
        Else
            Timerskill6.Stop()
        End If

    End Sub
    Private Sub TimerSkill7Tick(sender As Object, e As EventArgs) Handles TimerSkill7.Tick
        If (check7.Checked And skill7.Text <> String.Empty) Then
            If (CInt(skill7.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_7, 0)
            Else
                TimerSkill7.Stop()
            End If
        Else
            TimerSkill7.Stop()
        End If

    End Sub
    Private Sub TimerSkill8Tick(sender As Object, e As EventArgs) Handles Timerskill8.Tick
        If (check8.Checked And skill8.Text <> String.Empty) Then
            If (CInt(skill8.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_8, 0)
            Else
                Timerskill8.Stop()
            End If
        Else
            Timerskill8.Stop()
        End If

    End Sub
    Private Sub TimerSkill9Tick(sender As Object, e As EventArgs) Handles TimerSkill9.Tick
        If (check9.Checked And skill9.Text <> String.Empty) Then
            If (CInt(skill9.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_9, 0)
            Else
                TimerSkill9.Stop()
            End If
        Else
            TimerSkill9.Stop()
        End If

    End Sub
    Private Sub TimerSkill0Tick(sender As Object, e As EventArgs) Handles Timerskill0.Tick
        If (check0.Checked And skill0.Text <> String.Empty) Then
            If (CInt(skill0.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_0, 0)
            Else
                Timerskill0.Stop()
            End If
        Else
            Timerskill0.Stop()
        End If

    End Sub
    Private Sub TimerSkillF1_Tick(sender As Object, e As EventArgs) Handles TimerSkillF1.Tick
        If (checkf1.Checked And skillf1.Text <> String.Empty) Then
            If (CInt(skillf1.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F1, 0)
            Else
                TimerSkillF1.Stop()
            End If
        Else
            TimerSkillF1.Stop()
        End If

    End Sub
    Private Sub TimerSkillF2_Tick(sender As Object, e As EventArgs) Handles TimerskillF2.Tick
        If (checkf2.Checked And skillf2.Text <> String.Empty) Then
            If (CInt(skillf2.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F2, 0)
            Else
                TimerskillF2.Stop()
            End If
        Else
            TimerskillF2.Stop()
        End If

    End Sub
    Private Sub TimerSkillF3_Tick(sender As Object, e As EventArgs) Handles TimerSkillF3.Tick
        If (checkf3.Checked And skillf3.Text <> String.Empty) Then
            If (CInt(skillf3.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F3, 0)
            Else
                TimerSkillF3.Stop()
            End If
        Else
            TimerSkillF3.Stop()
        End If

    End Sub
    Private Sub TimerSkillF4_Tick(sender As Object, e As EventArgs) Handles TimerskillF4.Tick
        If (checkf4.Checked And skillf4.Text <> String.Empty) Then
            If (CInt(skillf4.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F4, 0)
            Else
                TimerskillF4.Stop()
            End If
        Else
            TimerskillF4.Stop()
        End If

    End Sub
    Private Sub TimerSkillF5_Tick(sender As Object, e As EventArgs) Handles TimerSkillF5.Tick
        If (checkf5.Checked And skillf5.Text <> String.Empty) Then
            If (CInt(skillf5.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F5, 0)
            Else
                TimerSkillF5.Stop()
            End If
        Else
            TimerSkillF5.Stop()
        End If

    End Sub
    Private Sub TimerSkillF6_Tick(sender As Object, e As EventArgs) Handles TimerskillF6.Tick
        If (checkf6.Checked And skillf6.Text <> String.Empty) Then
            If (CInt(skillf6.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F6, 0)
            Else
                TimerskillF6.Stop()
            End If
        Else
            TimerskillF6.Stop()
        End If

    End Sub
    Private Sub TimerSkillF7_Tick(sender As Object, e As EventArgs) Handles TimerSkillF7.Tick
        If (checkf7.Checked And skillf7.Text <> String.Empty) Then
            If (CInt(skillf7.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F7, 0)
            Else
                TimerSkillF7.Stop()
            End If
        Else
            TimerSkillF7.Stop()
        End If

    End Sub
    Private Sub TimerSkillF8_Tick(sender As Object, e As EventArgs) Handles TimerskillF8.Tick
        If (checkf8.Checked And skillf8.Text <> String.Empty) Then
            If (CInt(skillf8.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F8, 0)
            Else
                TimerskillF8.Stop()
            End If
        Else
            TimerskillF8.Stop()
        End If

    End Sub
    Private Sub TimerSkillF9_Tick(sender As Object, e As EventArgs) Handles TimerSkillF9.Tick
        If (checkf9.Checked And skillf9.Text <> String.Empty) Then
            If (CInt(skillf9.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F9, 0)
            Else
                TimerSkillF9.Stop()
            End If
        Else
            TimerSkillF9.Stop()
        End If

    End Sub
    Private Sub TimerSkillF10_Tick(sender As Object, e As EventArgs) Handles TimerskillF10.Tick
        If (checkf10.Checked And skillf10.Text <> String.Empty) Then
            If (CInt(skillf10.Text) >= 1) Then
                SendMessage(handle, &H100, KEY_F10, 0)
            Else
                TimerskillF10.Stop()
            End If
        Else
            TimerskillF10.Stop()
        End If

    End Sub

    Private Sub checkboxmago_CheckedChanged(sender As Object, e As EventArgs) Handles checkboxmago.CheckedChanged
        If checkboxmago.Checked Then
            skillr.Text = "0"
            skillr.Enabled = False
        Else
            skillr.Text = "1"
            skillr.Enabled = True
        End If
    End Sub

#End Region

#Region "Guardar"
    Private Sub guardararchivo(sender As Object, e As EventArgs) Handles Button4.Click
        Dim fs As FileStream

        Try

            If (TextBox1.Text = String.Empty) Then
                MsgBox("Debe indicar un nombre para el archivo a guardar.", MsgBoxStyle.Information, "Guardar")
                Exit Sub
            End If
            If File.Exists(ruta) Then
                fs = File.Create(ruta & TextBox1.Text & ".txt")
                fs.Close()
            Else
                Directory.CreateDirectory(ruta)
                fs = File.Create(ruta & TextBox1.Text & ".txt")
                fs.Close()
            End If

            Dim escribir As New StreamWriter(ruta & TextBox1.Text & ".txt")
            Try

                Dim linea As String = obtenerdata()
                escribir.WriteLine(linea)
                escribir.Close()
                MsgBox("Archivo " & TextBox1.Text & " creado correctamente", MsgBoxStyle.Information, "Guardar Presets")
            Catch ex As Exception
                MsgBox("Se presento un problema al escribir en el archivo: " & ex.Message, MsgBoxStyle.Critical, "Guardar Presets")
            End Try

        Catch ex As Exception
            MsgBox("Se presento un problema al momento de crear el archivo: " & ex.Message, MsgBoxStyle.Critical, "Guardar Presets")
        End Try
    End Sub


    Public Function obtenerdata() As String

        Dim linea As String

        If (checkbasico.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If

        If (checkboxmago.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If


        linea = linea & skille.Text & ","
        linea = linea & skillr.Text & ","


        If (CheckBoxrecojer.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If

        linea = linea & TextBoxrecojer.Text & ","

        linea = linea & skill1.Text & ","
        linea = linea & skill2.Text & ","
        linea = linea & skill3.Text & ","
        linea = linea & skill4.Text & ","
        linea = linea & skill5.Text & ","
        linea = linea & skill6.Text & ","
        linea = linea & skill7.Text & ","
        linea = linea & skill8.Text & ","
        linea = linea & skill9.Text & ","
        linea = linea & skill0.Text & ","


        If (check1.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (check2.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (check3.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (check4.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (check5.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (check6.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (check7.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (check8.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (check9.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (check0.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If


        linea = linea & skillf1.Text & ","
        linea = linea & skillf2.Text & ","
        linea = linea & skillf3.Text & ","
        linea = linea & skillf4.Text & ","
        linea = linea & skillf5.Text & ","
        linea = linea & skillf6.Text & ","
        linea = linea & skillf7.Text & ","
        linea = linea & skillf8.Text & ","
        linea = linea & skillf9.Text & ","
        linea = linea & skillf10.Text & ","

        If (checkf1.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (checkf2.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (checkf3.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (checkf4.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (checkf5.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (checkf6.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (checkf7.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (checkf8.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (checkf9.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If
        If (checkf10.Checked) Then
            linea = linea & "1,"
        Else
            linea = linea & "0,"
        End If

        Return linea


    End Function

    Private Sub cargar_archivo(sender As Object, e As EventArgs) Handles Button5.Click
        Dim linea As String = String.Empty
        Dim result As String
        Dim openFileDialog1 As System.Windows.Forms.OpenFileDialog

        openFileDialog1 = New System.Windows.Forms.OpenFileDialog()

        openFileDialog1.Filter = "(*.txt) |*.txt|(*.*) |*.*"

        If Not File.Exists(Application.StartupPath & "\presets") Then
            Directory.CreateDirectory(ruta)
        End If

        openFileDialog1.InitialDirectory = Application.StartupPath & "\presets"

        If openFileDialog1.ShowDialog() = DialogResult.OK Then
            result = openFileDialog1.FileName
        Else
            Exit Sub
        End If


        Dim leer As New StreamReader(result)

        Try

            While leer.Peek <> -1
                linea = leer.ReadLine()
                If String.IsNullOrEmpty(linea) Then
                    Continue While
                End If
            End While

            leer.Close()

            If linea <> String.Empty Then

                Dim data As String() = linea.Split(New Char() {","c})

                If data.Length > 0 Then


                    If (data(0).ToString() = "1") Then
                        checkbasico.Checked = True
                    Else
                        checkbasico.Checked = False
                    End If

                    If (data(1).ToString() = "1") Then
                        checkboxmago.Checked = True
                    Else
                        checkboxmago.Checked = False
                    End If

                    skille.Text = data(2).ToString()
                    skillr.Text = data(3).ToString()


                    If (data(4).ToString() = "1") Then
                        CheckBoxrecojer.Checked = True
                    Else
                        CheckBoxrecojer.Checked = False
                    End If

                    TextBoxrecojer.Text = data(5).ToString()

                    skill1.Text = data(6).ToString()
                    skill2.Text = data(7).ToString()
                    skill3.Text = data(8).ToString()
                    skill4.Text = data(9).ToString()
                    skill5.Text = data(10).ToString()
                    skill6.Text = data(11).ToString()
                    skill7.Text = data(12).ToString()
                    skill8.Text = data(13).ToString()
                    skill9.Text = data(14).ToString()
                    skill0.Text = data(15).ToString()


                    If (data(16).ToString() = "1") Then
                        check1.Checked = True
                    Else
                        check1.Checked = False
                    End If
                    If (data(17).ToString() = "1") Then
                        check2.Checked = True
                    Else
                        check2.Checked = False
                    End If
                    If (data(18).ToString() = "1") Then
                        check3.Checked = True
                    Else
                        check3.Checked = False
                    End If
                    If (data(19).ToString() = "1") Then
                        check4.Checked = True
                    Else
                        check4.Checked = False
                    End If
                    If (data(20).ToString() = "1") Then
                        check5.Checked = True
                    Else
                        check5.Checked = False
                    End If
                    If (data(21).ToString() = "1") Then
                        check6.Checked = True
                    Else
                        check6.Checked = False
                    End If
                    If (data(22).ToString() = "1") Then
                        check7.Checked = True
                    Else
                        check7.Checked = False
                    End If
                    If (data(23).ToString() = "1") Then
                        check8.Checked = True
                    Else
                        check8.Checked = False
                    End If
                    If (data(24).ToString() = "1") Then
                        check9.Checked = True
                    Else
                        check9.Checked = False
                    End If
                    If (data(25).ToString() = "1") Then
                        check0.Checked = True
                    Else
                        check0.Checked = False
                    End If


                    skillf1.Text = data(26).ToString()
                    skillf2.Text = data(27).ToString()
                    skillf3.Text = data(28).ToString()
                    skillf4.Text = data(29).ToString()
                    skillf5.Text = data(30).ToString()
                    skillf6.Text = data(31).ToString()
                    skillf7.Text = data(32).ToString()
                    skillf8.Text = data(33).ToString()
                    skillf9.Text = data(34).ToString()
                    skillf10.Text = data(35).ToString()


                    If (data(36).ToString() = "1") Then
                        checkf1.Checked = True
                    Else
                        checkf1.Checked = False
                    End If
                    If (data(37).ToString() = "1") Then
                        checkf2.Checked = True
                    Else
                        checkf2.Checked = False
                    End If
                    If (data(38).ToString() = "1") Then
                        checkf3.Checked = True
                    Else
                        checkf3.Checked = False
                    End If
                    If (data(39).ToString() = "1") Then
                        checkf4.Checked = True
                    Else
                        checkf4.Checked = False
                    End If
                    If (data(40).ToString() = "1") Then
                        checkf5.Checked = True
                    Else
                        checkf5.Checked = False
                    End If
                    If (data(41).ToString() = "1") Then
                        checkf6.Checked = True
                    Else
                        checkf6.Checked = False
                    End If
                    If (data(42).ToString() = "1") Then
                        checkf7.Checked = True
                    Else
                        checkf7.Checked = False
                    End If
                    If (data(43).ToString() = "1") Then
                        checkf8.Checked = True
                    Else
                        checkf8.Checked = False
                    End If
                    If (data(44).ToString() = "1") Then
                        checkf9.Checked = True
                    Else
                        checkf9.Checked = False
                    End If
                    If (data(45).ToString() = "1") Then
                        checkf10.Checked = True
                    Else
                        checkf10.Checked = False
                    End If


                End If


            End If

        Catch ex As Exception
            MsgBox("Se presento un problema al leer el archivo: " & ex.Message, MsgBoxStyle.Critical, "Cargar Archivo")
        End Try



    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("http://www.instagram.com/jilmy.daccarett/")
    End Sub


#End Region

End Class
