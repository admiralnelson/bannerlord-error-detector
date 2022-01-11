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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ErrorWindow))
        Me.widget = New System.Windows.Forms.WebBrowser()
        Me.widgetMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.widgetMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'widget
        '
        Me.widget.Dock = System.Windows.Forms.DockStyle.Fill
        Me.widget.Location = New System.Drawing.Point(0, 0)
        Me.widget.Margin = New System.Windows.Forms.Padding(10, 9, 10, 9)
        Me.widget.MinimumSize = New System.Drawing.Size(63, 57)
        Me.widget.Name = "widget"
        Me.widget.Size = New System.Drawing.Size(3765, 1301)
        Me.widget.TabIndex = 3
        '
        'widgetMenu
        '
        Me.widgetMenu.ImageScalingSize = New System.Drawing.Size(48, 48)
        Me.widgetMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyToolStripMenuItem, Me.SelectAllToolStripMenuItem, Me.SaveToolStripMenuItem})
        Me.widgetMenu.Name = "widgetMenu"
        Me.widgetMenu.Size = New System.Drawing.Size(247, 172)
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(246, 56)
        Me.CopyToolStripMenuItem.Text = "Copy"
        '
        'SelectAllToolStripMenuItem
        '
        Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(246, 56)
        Me.SelectAllToolStripMenuItem.Text = "Select All"
        '
        'SaveToolStripMenuItem
        '
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        Me.SaveToolStripMenuItem.Size = New System.Drawing.Size(246, 56)
        Me.SaveToolStripMenuItem.Text = "Save"
        '
        'ErrorWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(19.0!, 37.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(3765, 1301)
        Me.ControlBox = False
        Me.Controls.Add(Me.widget)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(10, 9, 10, 9)
        Me.Name = "ErrorWindow"
        Me.Text = "ErrorWindow"
        Me.widgetMenu.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents widget As Windows.Forms.WebBrowser
    Friend WithEvents widgetMenu As Windows.Forms.ContextMenuStrip
    Friend WithEvents CopyToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SelectAllToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents SaveToolStripMenuItem As Windows.Forms.ToolStripMenuItem
End Class
