Imports TaleWorlds.CampaignSystem
Imports TaleWorlds.Core
Imports TaleWorlds.MountAndBlade
Imports System.Diagnostics
Imports TaleWorlds.CampaignSystem.SandBox.GameComponents
Imports System.Threading

Namespace Global.MyVBProject
    Public Class MySubModule
        Inherits MBSubModuleBase

        Protected Overrides Sub OnSubModuleLoad()
            MyBase.OnSubModuleLoad()
        End Sub

        Public Overrides Sub OnGameEnd(game As Game)
            MyBase.OnGameEnd(game)
        End Sub

        Protected Overrides Sub OnGameStart(game As Game, gameStarterObject As IGameStarter)
            Dim campaign = game.GameType
            If (campaign Is Nothing) Then
                Debug.WriteLine("oops!")
                Exit Sub
            End If
            Dim campaignStarter = CType(gameStarterObject, CampaignGameStarter)
                AddBehaviour(campaignStarter)
        End Sub

        Protected Overrides Sub OnBeforeInitialModuleScreenSetAsRoot()
            MyBase.OnBeforeInitialModuleScreenSetAsRoot()
            Dim ver = System.Environment.Version
            InformationManager.ShowInquiry(New InquiryData(
                "Net Enviroment",
                $"running on version {ver}",
                True,
                False,
                "Accept",
                "",
                Sub()
                    'Environment.Exit(1)
                End Sub,
                Sub()

                End Sub))
        End Sub

        Private Sub AddBehaviour(gameInit As CampaignGameStarter)
            gameInit.AddBehavior(New SimpleDayCounter)
        End Sub

    End Class

End Namespace