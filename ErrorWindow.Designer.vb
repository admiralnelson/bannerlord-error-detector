<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ErrorWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.widget = New System.Windows.Forms.WebBrowser()
        Me.SuspendLayout()
        '
        'widget
        '
        Me.widget.Dock = System.Windows.Forms.DockStyle.Fill
        Me.widget.Location = New System.Drawing.Point(0, 0)
        Me.widget.MinimumSize = New System.Drawing.Size(20, 20)
        Me.widget.Name = "widget"
        Me.widget.Size = New System.Drawing.Size(1189, 457)
        Me.widget.TabIndex = 3
        '
        'ErrorWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1189, 457)
        Me.ControlBox = False
        Me.Controls.Add(Me.widget)
        Me.Name = "ErrorWindow"
        Me.Text = "ErrorWindow"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents widget As Windows.Forms.WebBrowser
End Class
