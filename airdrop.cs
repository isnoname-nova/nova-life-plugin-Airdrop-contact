using BestHTTP;
using BestHTTP.Extensions;
using Crosstales.Common.Util;
using Dissonance.Config;
using JetBrains.Annotations;
using Life;
using Life.AreaSystem;
using Life.DB;
using Life.Network;
using Life.UI;
using Life.VehicleSystem;
using Mirror;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class airdropcommande : Plugin
{
    private LifeServer server;
    private string airdropPath;
   

    public airdropcommande(IGameAPI api) : base(api)
    {

    }

    public override void OnPlayerSpawnCharacter(Player player, NetworkConnection conn, Characters character)
    {
        base.OnPlayerSpawnCharacter(player, conn, character);

        string playerId = player.character.Id.ToString();
        string[] lines = File.ReadAllLines(airdropPath);





        bool playerExists = false;
        int playerIndex = -1; 
        for (int i = 0; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length == 2 && parts[0] == playerId)
            {
                playerExists = true;
                playerIndex = i; 
            }
        }

        if (!playerExists)
        {
            string playerName = $"iphone de - {player.character.Firstname}";
            string newLine = $"{playerId},{playerName}";
            File.AppendAllLines(airdropPath, new[] { newLine });
        }



    }

    public override void OnPluginInit()
    {
        base.OnPluginInit();
        airdropPath = Path.Combine(this.pluginsPath, "airdrop.txt");
        this.server = Nova.server;
        Debug.Log(airdropPath);
        LifeServer server = this.server;
        _ = server.OnMinutePassedEvent;



        if (!File.Exists(airdropPath))
        {
            File.Create(airdropPath);
        }

        {
            new SChatCommand("/airdropname", "Modifie le nom de téléphone dans le fichier airdrop.txt", "/airdropname [nom]", (player, args) =>
            {
                
                
                UIPanel numberPanel = new UIPanel($"Changement de nom de telephone", UIPanel.PanelType.Input)
                        .AddButton("Fermer", (ui) =>
                        {
                            player.ClosePanel(ui);
                        })
                        .SetInputPlaceholder("nom.")
                        .AddButton("Valider", (ui) =>
                        {
                            string name = ui.inputText;

                            if (string.IsNullOrEmpty(name))
                            {
                                
                                return;
                            }

                            if (ui.inputText.Contains(","))
                            {
                                player.SendText(string.Format("<color={0}>Le nom ne doit pas contenir de virgule.</color>", LifeServer.COLOR_RED));
                                return;
                            }



                            string playerId = player.character.Id.ToString();
                string[] lines = File.ReadAllLines(airdropPath);

                bool playerExists = false;
                int playerIndex = -1; 
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(',');
                    if (parts.Length == 2 && parts[0] == playerId)
                    {
                        playerExists = true;
                        playerIndex = i; 
                    }
                }

                if (!playerExists)
                {
                    string newLine = $"{playerId},{name}";
                    File.AppendAllLines(airdropPath, new[] { newLine });
                }
                else
                {
                    lines[playerIndex] = $"{playerId},{name}";
                    File.WriteAllLines(airdropPath, lines);
                }

                player.SendText(string.Format("<color={0}>Le nom de téléphone associé au joueur '{1}' a été modifié en '{2}'.</color>", LifeServer.COLOR_GREEN, player.character.Firstname, name));

                            player.ClosePanel(ui);
                        });
                player.ShowPanelUI(numberPanel);
            }).Register();
            new SChatCommand("/airdropuser", "Affiche le nom de téléphone associé à un joueur", "/airdropuser [playerId]", (player, args) =>
            {
                if (args.Length != 1)
                {
                    player.SendText(string.Format("<color={0}>Erreur : vous devez spécifier un playerId.</color>", LifeServer.COLOR_RED));

                    return;
                }

                string playerId = args[0];
                string[] lines = File.ReadAllLines(airdropPath);

                bool playerExists = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split(',');
                    if (parts.Length == 2 && parts[0] == playerId)
                    {
                        player.SendText(string.Format("<color={0}>Le nom de téléphone associé au joueur '{1}' est '{2}'.</color>", LifeServer.COLOR_GREEN, playerId, parts[1]));

                        playerExists = true;
                        break;
                    }
                }

                if (!playerExists)
                {
                    player.SendText(string.Format("<color={0}>Le joueur '{1}' n'a pas de nom de téléphone associé.</color>", LifeServer.COLOR_RED, playerId));

                }


            }).Register();



        }
    }
    public override void OnPlayerInput(Player player, KeyCode keyCode, bool onUI)
    {
        base.OnPlayerInput(player, keyCode, onUI);

        if (keyCode != KeyCode.Alpha8 || onUI)
            return;
        Debug.Log("Rp");
        Player target = player.GetClosestPlayer();

        if (target != null)
        {
            string playerid = player.character.Id.ToString();

            string[] lines = File.ReadAllLines(airdropPath);

            string targetId = target.character.Id.ToString();

            
            


            for (int i = 0; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length == 2 && parts[0] == targetId)
                {
                    Debug.Log(targetId);
                    string targetPhoneName = parts[1];
                    Debug.Log(targetPhoneName);
                    
                    this.server.GetCountOnlineBiz(1, true);
                    string phoneName2 = null;
                    foreach (string line in lines)
                    {
                        string[] parts2 = line.Split(',');
                        if (parts2.Length == 2 && parts2[0] == playerid)
                        {
                            phoneName2 = line;
                            break;
                        }
                    }
                    Debug.Log(phoneName2 + "phoneName2");
                    
                    string[] parts4 = phoneName2.Split(',');
                    string phoneName22 = parts4[1];
                    Debug.Log(phoneName22 + "phoneName2");
                    
                    server.SendLocalText(phoneName22 + " a demmndé a faire un airdrop de contact avec " + targetPhoneName, 2f, player.setup.transform.position);
                    string message2 = string.Format(" Téléphone {0} vous demande de l'ajouter en contact  via airdrop", phoneName22);
                    
                    UIPanel panel = new UIPanel("Airdrop", UIPanel.PanelType.Text)
                        .AddButton("Accepter", (Action<UIPanel>)(ui =>
                        {

                            
                            target.SendText("Airdrop en cours...");
                            LifeDB.CreateContact(player.character.Id, target.character.PhoneNumber, targetPhoneName);
                            LifeDB.CreateContact(target.character.Id, player.character.PhoneNumber, phoneName22);
                            target.SendText("<color=green>contact ajouté " + player.character.PhoneNumber + ":" + phoneName22 + "</color>");
                            player.SendText("<color=green>contact ajouté " + target.character.PhoneNumber + ":" + targetPhoneName + "</color>");
                            long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                            string messageair = string.Format("automatique message de airdrop de \n{0}:\n\n{1}\nProtocole V1.0.0.0 BY IS-NO-NAME", phoneName22, player.character.PhoneNumber);
                            string messageair2 = string.Format("automatique message de airdrop de \n{0}:\n{1}\nProtocole V1.0.0.0 BY IS-NO-NAME", targetPhoneName, target.character.PhoneNumber);
                            LifeDB.SendSMS(target.character.Id, player.character.PhoneNumber, target.character.PhoneNumber, unixTimeSeconds, messageair);
                            LifeDB.SendSMS(player.character.Id, target.character.PhoneNumber, player.character.PhoneNumber, unixTimeSeconds, messageair2);
                            player.Save();
                            target.Save();
                            server.SendLocalText("<color=green>" +  targetPhoneName + " a effectué un airdrop de contact avec " + phoneName22 + "</color>", 2f, player.setup.transform.position);
                            target.ClosePanel(ui);
                        }))
                        .AddButton("<color=red>Refuser", (Action<UIPanel>)(ui =>
                        {
                            server.SendLocalText(phoneName22 + " <color=red> a refusé </color> de effectuer un airdrop de contact avec " + targetPhoneName, 2f, target.setup.transform.position);

                            target.ClosePanel(ui);
                        }))
                        .SetText(message2);
                    target.ShowPanelUI(panel);
                }
               
                }
            }
             else
            {
                player.SendText($"aucun telephone trouvé.");

            }
    }
}

