<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.bgwUpdater = New System.ComponentModel.BackgroundWorker()
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.lblLocalVersion = New System.Windows.Forms.Label()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.prgbarUpdate = New System.Windows.Forms.ProgressBar()
        Me.lblError = New System.Windows.Forms.Label()
        Me.lblProgress = New System.Windows.Forms.Label()
        Me.SuspendLayout()
        '
        'bgwUpdater
        '
        Me.bgwUpdater.WorkerReportsProgress = True
        Me.bgwUpdater.WorkerSupportsCancellation = True
        '
        'lblVersion
        '
        Me.lblVersion.BackColor = System.Drawing.Color.Transparent
        Me.lblVersion.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!)
        Me.lblVersion.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblVersion.Location = New System.Drawing.Point(199, 206)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(195, 29)
        Me.lblVersion.TabIndex = 0
        Me.lblVersion.Text = "Version: 0.0.0"
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblLocalVersion
        '
        Me.lblLocalVersion.BackColor = System.Drawing.Color.Transparent
        Me.lblLocalVersion.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLocalVersion.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblLocalVersion.Location = New System.Drawing.Point(5, 206)
        Me.lblLocalVersion.Name = "lblLocalVersion"
        Me.lblLocalVersion.Size = New System.Drawing.Size(195, 29)
        Me.lblLocalVersion.TabIndex = 1
        Me.lblLocalVersion.Text = "Local Version: 0.0.0"
        Me.lblLocalVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnUpdate
        '
        Me.btnUpdate.Enabled = False
        Me.btnUpdate.Location = New System.Drawing.Point(306, 174)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(88, 29)
        Me.btnUpdate.TabIndex = 2
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'prgbarUpdate
        '
        Me.prgbarUpdate.Location = New System.Drawing.Point(96, 174)
        Me.prgbarUpdate.Name = "prgbarUpdate"
        Me.prgbarUpdate.Size = New System.Drawing.Size(204, 29)
        Me.prgbarUpdate.TabIndex = 3
        Me.prgbarUpdate.Visible = False
        '
        'lblError
        '
        Me.lblError.BackColor = System.Drawing.Color.Transparent
        Me.lblError.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblError.ForeColor = System.Drawing.Color.FromArgb(CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.lblError.Location = New System.Drawing.Point(9, -1)
        Me.lblError.Name = "lblError"
        Me.lblError.Size = New System.Drawing.Size(385, 44)
        Me.lblError.TabIndex = 4
        Me.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblProgress
        '
        Me.lblProgress.AutoEllipsis = True
        Me.lblProgress.BackColor = System.Drawing.Color.Transparent
        Me.lblProgress.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!)
        Me.lblProgress.ForeColor = System.Drawing.Color.Black
        Me.lblProgress.Location = New System.Drawing.Point(12, 91)
        Me.lblProgress.Name = "lblProgress"
        Me.lblProgress.Size = New System.Drawing.Size(376, 51)
        Me.lblProgress.TabIndex = 6
        Me.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(400, 240)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.lblProgress)
        Me.Controls.Add(Me.lblError)
        Me.Controls.Add(Me.prgbarUpdate)
        Me.Controls.Add(Me.lblLocalVersion)
        Me.Controls.Add(Me.lblVersion)
        Me.DoubleBuffered = True
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(416, 279)
        Me.MinimumSize = New System.Drawing.Size(416, 279)
        Me.Name = "frmMain"
        Me.Text = "Updater"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bgwUpdater As System.ComponentModel.BackgroundWorker
    Friend WithEvents lblVersion As Label
    Friend WithEvents lblLocalVersion As Label
    Friend WithEvents btnUpdate As Button
    Friend WithEvents prgbarUpdate As ProgressBar
    Friend WithEvents lblError As Label
    Friend WithEvents lblProgress As Label
End Class
