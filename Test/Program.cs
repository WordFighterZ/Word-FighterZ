using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace Test
{
    #region Musique
    class Music //Class qui sera executer dans un thread différent, permettant à la musique de ne jamais être mise en pause, et de se répéter entre les parties
    {
        public void MusicRepeat() 
        {
            string play_string = @"Music.mp3"; //On met dans une variable le son
            Thread.CurrentThread.IsBackground = true;
            
            while (true)
            {
                var reader = new NAudio.Wave.Mp3FileReader(play_string); //On prépare la lecture
                var waveOut = new NAudio.Wave.WaveOut(); // or WaveOutEvent()
                waveOut.Init(reader); //On initialise le lecteur
                waveOut.Play(); //On joue le son
                while (waveOut.PlaybackState != NAudio.Wave.PlaybackState.Stopped) ; //Tant que la musique n'est pas finie, ne rien faire, permet de repéter la musique, sans quelle ne se joue plusieurs fois en même temps
            }
        }
    }
    #endregion

    class Program
    {
        
        static void Main(string[] args)
        {
            try //Si ce n'est pas la première boucle, on ne fera alors rien
            {
                if (args[0] != "1") 
                {
                    var music = new Music();
                    Thread thr = new Thread(music.MusicRepeat);
                    thr.Start();
                }
            }
            catch
            {
                var music = new Music();
                Thread thr = new Thread(music.MusicRepeat);
                thr.Start();
            }

            SetCurrentFont("Consolas", 30); //Grossissement de la police d'écriture
            ShowWindow(ThisConsole, MAXIMIZE); //Permet l'affichage en plein écran

            Console.OutputEncoding = System.Text.Encoding.Unicode; //Changement d'encodage, pour l'affichage des symboles/emojis

            Console.Title = "Word FighterZ"; //Changement du nom de la console


            //Affichage du titre du jeu
            Title();


            Random aleatoire = new Random();

            //Déclaration des variables
            string joueurRole;
            string ordiRole;
            int joueurVies;
            int ordiVies;
            int jokerVies = aleatoire.Next(1, 5);
            bool isAttack = true;
            bool isAttackOrdi = true;
            List<string> listeRole = new List<string> { "Damager", "Healer", "Tanker", "Joker" };

            //Attribution des dégâts par rapport aux rôles
            var charactersRoles = new Dictionary<int, string>() { { 1, "D" }, { 2, "H" }, { 3, "T" }, { 4, "J" } };

            //Attribution des points de vie par rapport aux rôles
            var PvRoles = new Dictionary<string, int>() { { "D", 3 }, { "H", 4 }, { "T", 5 }, { "J", jokerVies } };

            //Choix du joueur pour son rôle
            joueurRole = charactersRoles[PlayerChoice("Combatant, choisisez votre spécialisation! \n", new List<string> { "⚔ Damager", "✸ Healer", "⛨ Tanker", "♠ Joker" }, "Vous avez choisi d'être un {0}. Fort bien !", "Select.mp3", true)];
            //Choix du rôle pour l'IA de façon aléatoire
            int indexRandRole = aleatoire.Next(1, 5);
            ordiRole = charactersRoles[indexRandRole];

            Console.SetCursorPosition((Console.WindowWidth - (new string("L'ordinateur jouera lui un .\n").Length + listeRole[indexRandRole - 1].Length)) / 2, Console.CursorTop); //Affichage des rôles choisis
            Console.WriteLine("L'ordinateur jouera lui un {0}.\n", listeRole[indexRandRole - 1]);

            //Choix du niveau de l'ordinateur
            int niveauIA = PlayerChoice("Choisissez le niveau de l'ordinateur.\n", new List<string> { "Patate", "Qualifié" }, "Vous avez choisi {0}", "Select.mp3", false);

            PlayerChoice("", new List<string> { "Commencer le Combat !" }, "", "Select.mp3", false);
            Console.Clear();
            Title();


            //Ajout des points de vie par rapport aux rôles
            joueurVies = PvRoles[joueurRole];
            ordiVies = PvRoles[ordiRole];

            //boucle du jeu
            while (joueurVies > 0 && ordiVies > 0)
            {
                AffichageVie(joueurRole, ordiRole, joueurVies, ordiVies); //Affichage de la barre de vie

                //Choix de l'action du joueur
                if (isAttack) //Si le joueur peut encore faire sa capacité spéciale
                {
                    var action = Tuple.Create(0,0);

                    if (niveauIA == 0) //Si l'ordi et en "patate"
                    {
                        if (isAttackOrdi) //Si l'ordi peut encore faire sa capacité spéciale
                        {
                            action = ResolutionAction(PlayerChoice("\n", new List<string> { "⚔ Attaque", "⛨ Défense", "✸ Capacité Spéciale" }, "", "Select.mp3", false), int.Parse(IaRandomChoice(new List<string> { "1", "2", "3" })), joueurRole, ordiRole, joueurVies, ordiVies, ref isAttack, ref isAttackOrdi); //On résout l'action donnée par le joueur et le choix de l'IA
                        }
                        else //Sinon
                        {
                            action = ResolutionAction(PlayerChoice("\n", new List<string> { "⚔ Attaque", "⛨ Défense", "✸ Capacité Spéciale" }, "", "Select.mp3", false), int.Parse(IaRandomChoice(new List<string> { "1", "2" })), joueurRole, ordiRole, joueurVies, ordiVies, ref isAttack, ref isAttackOrdi); //On résout l'action donnée par le joueur et le choix de l'IA
                        }
                    }
                    else //Sinon
                    {
                        action = ResolutionAction(PlayerChoice("\n", new List<string> { "⚔ Attaque", "⛨ Défense", "✸ Capacité Spéciale" }, "", "Select.mp3", false), ChoixIANormal(ordiRole, joueurRole, ordiVies, joueurVies, ref isAttackOrdi, ref isAttack), joueurRole, ordiRole, joueurVies, ordiVies, ref isAttack, ref isAttackOrdi); //On résout l'action donnée par le joueur et le choix de l'IA
                    }

                    joueurVies += action.Item1; //On applique les changements de points de vie
                    ordiVies += action.Item2;

                    //Affichage des pv perdus/gagnés
                    if (action.Item1 > 0)
                    {
                        AffichageLine("Le joueur à gagné " + action.Item1 + " pv !");
                    }
                    else if (action.Item1 < 0)
                    {
                        AffichageLine("Le joueur à perdu " + action.Item1 + " pv !");
                    }
                    else
                    {
                        AffichageLine("Le joueur n'a perdu aucun pv !");
                    }

                    if (action.Item2 > 0)
                    {
                        AffichageLine("L'Ordinateur à gagné " + action.Item2 + " pv !");
                    }
                    else if (action.Item2 < 0)
                    {
                        AffichageLine("L'Ordinateur à perdu " + action.Item2 + " pv !");
                    }
                    else
                    {
                        AffichageLine("L'Ordinateur n'a perdu aucun pv !");
                    }

                }
                else //Si le joueur n'a plus son attaque spéciales
                {
                    var action = Tuple.Create(0, 0);
                    if (niveauIA == 0)
                    {
                        if (isAttackOrdi)
                        {
                            action = ResolutionAction(PlayerChoice("\n", new List<string> { "⚔ Attaque", "⛨ Défense" }, "", "Select.mp3", false), int.Parse(IaRandomChoice(new List<string> { "1", "2", "3" })), joueurRole, ordiRole, joueurVies, ordiVies, ref isAttack, ref isAttackOrdi); //On résout l'action donnée par le joueur et le choix de l'IA
                        }
                        else
                        {
                            action = ResolutionAction(PlayerChoice("\n", new List<string> { "⚔ Attaque", "⛨ Défense" }, "", "Select.mp3", false), int.Parse(IaRandomChoice(new List<string> { "1", "2" })), joueurRole, ordiRole, joueurVies, ordiVies, ref isAttack, ref isAttackOrdi); //On résout l'action donnée par le joueur et le choix de l'IA
                        }
                    }
                    else
                    {
                        action = ResolutionAction(PlayerChoice("\n", new List<string> { "⚔ Attaque", "⛨ Défense" }, "", "Select.mp3", false), ChoixIANormal(ordiRole, joueurRole, ordiVies, joueurVies, ref isAttackOrdi, ref isAttack), joueurRole, ordiRole, joueurVies, ordiVies, ref isAttack, ref isAttackOrdi); //On résout l'action donnée par le joueur et le choix de l'IA
                    }
                    joueurVies += action.Item1;
                    ordiVies += action.Item2;

                    if (action.Item1 > 0) //Affichage des dégats perdu/gagné par le joueur
                    {
                        AffichageLine("Le joueur à gagné " + action.Item1 + " pv !");
                    }
                    else if (action.Item1 < 0)
                    {
                        AffichageLine("Le joueur à perdu " + action.Item1 + " pv !");
                    }
                    else
                    {
                        AffichageLine("Le joueur n'a perdu aucun pv !");
                    }

                    if (action.Item2 > 0) //Affichage des dégats perdu/gagné par l'Ordinateur
                    {
                        AffichageLine("L'Ordinateur à gagné " + action.Item2 + " pv !");
                    }
                    else if (action.Item2 < 0)
                    {
                        AffichageLine("L'Ordinateur à perdu " + action.Item2 + " pv !");
                    }
                    else
                    {
                        AffichageLine("L'Ordinateur n'a perdu aucun pv !");
                    }
                }

                

                if (joueurVies > 0 && ordiVies > 0) //Si le combat continu
                {
                    PlayerChoice("", new List<string> { "Continuer le combat !" }, "", "nothing.mp3", false);
                }
                else //Si le combat est fini
                {
                    PlayerChoice("", new List<string> { "Finir le combat !" }, "", "nothing.mp3", false);
                }
                Console.Clear(); //Préparation de l'affichage pour le prochain tour
                Title();

            }

            AffichageVie(joueurRole, ordiRole, joueurVies, ordiVies); //Affichage des points de vies finaux
            Console.WriteLine("\n\n");
            //Affichage de victoire 
            if (joueurVies <= 0 && ordiVies <= 0) //Si égalité
            {
                Console.ForegroundColor = ConsoleColor.Green;
                AffichageLine("██████  ██████   █████  ██     ██ ");
                Thread.Sleep(100);
                AffichageLine("██   ██ ██   ██ ██   ██ ██     ██ ");
                Thread.Sleep(100);
                AffichageLine("██   ██ ██████  ███████ ██  █  ██ ");
                Thread.Sleep(100);
                AffichageLine("██   ██ ██   ██ ██   ██ ██ ███ ██ ");
                Thread.Sleep(100);
                AffichageLine("██████  ██   ██ ██   ██  ███ ███  ");
                Thread.Sleep(100);
                Console.ForegroundColor = ConsoleColor.White;
                AffichageLine("En gros égalité quoi.");
            }

            else if (ordiVies <= 0) //Si victoire
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                AffichageLine(@" /$$     /$$  /$$$$$$  /$$   /$$       /$$      /$$ /$$$$$$ /$$   /$$");
                Thread.Sleep(100);
                AffichageLine(@"|  $$   /$$/ /$$__  $$| $$  | $$      | $$  /$ | $$|_  $$_/| $$$ | $$");
                Thread.Sleep(100);
                AffichageLine(@" \  $$ /$$/ | $$  \ $$| $$  | $$      | $$ /$$$| $$  | $$  | $$$$| $$");
                Thread.Sleep(100);
                AffichageLine(@"  \  $$$$/  | $$  | $$| $$  | $$      | $$/$$ $$ $$  | $$  | $$ $$ $$");
                Thread.Sleep(100);
                AffichageLine(@"   \  $$/   | $$  | $$| $$  | $$      | $$$$_  $$$$  | $$  | $$  $$$$");
                Thread.Sleep(100);
                AffichageLine(@"    | $$    | $$  | $$| $$  | $$      | $$$/ \  $$$  | $$  | $$\  $$$");
                Thread.Sleep(100);
                AffichageLine(@"    | $$    |  $$$$$$/|  $$$$$$/      | $$/   \  $$ /$$$$$$| $$ \  $$");
                Thread.Sleep(100);
                AffichageLine(@"    |__/     \______/  \______/       |__/     \__/|______/|__/  \__/");
                Thread.Sleep(100);
                Console.ForegroundColor = ConsoleColor.White;
            }

            else //Si défaite
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                AffichageLine(@"     )      )              (         )    (           ");
                Thread.Sleep(100);
                AffichageLine(@"  ( /(   ( /(              )\ )   ( /(    )\ )        ");
                Thread.Sleep(100);
                AffichageLine(@"  )\())  )\())      (     (()/(   )\())  (()/(   (    ");
                Thread.Sleep(100);
                AffichageLine(@" ((_)\  ((_)\       )\     /(_)) ((_)\    /(_))  )\   ");
                Thread.Sleep(100);
                AffichageLine(@"__ ((_)   ((_)   _ ((_)   (_))     ((_)  (_))   ((_)  ");
                Thread.Sleep(100);
                AffichageLine(@"\ \ / /  / _ \  | | | |   | |     / _ \  / __|  | __| ");
                Thread.Sleep(100);
                AffichageLine(@" \ V /  | (_) | | |_| |   | |__  | (_) | \__ \  | _|  ");
                Thread.Sleep(100);
                AffichageLine(@"  |_|    \___/   \___/    |____|  \___/  |___/  |___| ");
                Thread.Sleep(100);
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("\n\n");
            if (PlayerChoice("Souhaitez-vous refaire un duel ?", new List<string> { "Oui !", "Non..." }, "", "Select.mp3", false) == 1) //Demande de revanche
            {
                Console.Clear();
                Main(new string[]{"1"});
            }
        }

        static int PlayerChoice(string phraseChoix, List<string> listePhrase, string phraseConclu, string enterAudioFile, bool selecperso)
        {
            // Cette fonction permet au joueur de faire un choix avec les flèches, plutôt que de choisir en inscrivant un nombre.
            // Entrées : Cette fonction prend en fonction un string qui sera la question, un string qui sera les choix possible, une phrase de conclusion qui dépend du choix, et un audio qui se jouera lors de la selection du joueur
            // Sortie : int correspondant à l'index du choix, ce sera au programme principal de bien utiliser cet int


            AffichageLine(phraseChoix); //Affichage de la "question"
            int index = 1; //IL sera modifié, et indique l'option actuellement selectionnée
            int preindex = index;//Il garde en mémoire le précedent index, très important
            bool Condi = true; //La condition qui permet de sortir de la boucle

            //Affichage du menu de base
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Affichage(listePhrase[0]);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            if (selecperso)
            {
                AfficherStats(Console.CursorTop, index, new Tuple<int, int>(Console.CursorLeft, Console.CursorTop));
            }
            Console.WriteLine();
            for (int i = 1; i < listePhrase.Count(); i++)
            {
                AffichageLine(listePhrase[i]);
            }


            Console.SetCursorPosition((Console.WindowWidth - listePhrase[0].Length) / 2, Console.CursorTop); //On bouge le curseur au début du premier mot
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - (listePhrase.Count()));
            Console.CursorVisible = false; //On rend le curseur invisible

            var audioFile = new NAudio.Wave.AudioFileReader("nothing.mp3"); ; //On prépare la lecture des fichiers audio
            var outputDevice = new NAudio.Wave.WaveOutEvent(); ;

            while (Condi) //Tant que le joueur n'a pas fait de choix
            {
                var ch = Console.ReadKey(false).Key;
                switch (ch)
                {
                    case ConsoleKey.DownArrow: //Si la flèche du bas et pressée 
                        if (index < listePhrase.Count()) // Et que on est pas déjà au dernier choix possible
                        {
                            index += 1; //On augmente l'index
                            audioFile = new NAudio.Wave.AudioFileReader("blipSelect.wav"); //On joue le son du curseur qui bouge
                            outputDevice = new NAudio.Wave.WaveOutEvent();
                            outputDevice.Init(audioFile);
                            outputDevice.Play();
                        }
                        break;
                    case ConsoleKey.UpArrow: //Si la flèche du haut et pressée 
                        if (index > 1) // Et que on est pas au premier choix possible
                        {
                            index -= 1; //On décrémente l'index
                            audioFile = new NAudio.Wave.AudioFileReader("blipSelect.wav"); //On joue le son du curseur qui bouge
                            outputDevice = new NAudio.Wave.WaveOutEvent();
                            outputDevice.Init(audioFile);
                            outputDevice.Play();
                        }
                        break;
                    case ConsoleKey.Enter: //Si la touche Entrée et pressée 
                        Condi = false; //On ferme la boucle
                        ClearStats(Console.CursorTop, new Tuple<int, int>(Console.CursorLeft, Console.CursorTop));
                        audioFile = new NAudio.Wave.AudioFileReader(enterAudioFile); //On joue le son donnée en argument
                        outputDevice = new NAudio.Wave.WaveOutEvent();
                        outputDevice.Init(audioFile);
                        outputDevice.Play();
                        break;
                }
                if (Condi)
                {
                    //Affichage qui permet de savoir ou est le curseur

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Affichage(listePhrase[preindex - 1]);
                    Console.SetCursorPosition(0, Console.CursorTop - (preindex - index));
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Affichage(listePhrase[index - 1]);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    if (selecperso)
                    {
                        AfficherStats(Console.CursorTop, index, new Tuple<int, int>(Console.CursorLeft, Console.CursorTop));
                    }
                    preindex = index;
                }


            }


            for (int i = 0; i < 2 + listePhrase.Count() - index; i++) //On fait le bon nombre de saut de ligne pour afficher la phrase de conclusion au bon endroit
            {
                Console.Write("\n");
            }

            Console.SetCursorPosition((Console.WindowWidth - (phraseConclu.Length + listePhrase[index - 1].Length)) / 2, Console.CursorTop);
            Console.WriteLine(phraseConclu, listePhrase[index - 1]); //On affiche la phrase de conclusio ndu choix

            return index; //On retourne l'index du choix
        }

        static Tuple<int, int> ResolutionAction(int actionJoueur, int actionIA, string role, string roleordi, int joueurVies, int ordiVies, ref bool isAttack, ref bool isAttackOrdi)
        {
            //Cette fonction permet d'infliger le bon nombre de dégats selon les actions choisies par l'IA et par le joueur
            //Entrées : L'action du joueur, de l'IA, le rôle du joueur, de l'IA, le nombre de pv du joueur, de l'IA, et les bool indiquant si les joueurs peuvent encore faire leurs coups spéciaux, en ref.
            //Sortie : Un tuple qui précise un opération à faire sur les pv de chaque joueurs


            Random aleatoire = new Random();
            int degatsJoueur = 0; //Initialisation des dégats de chaque joueurs
            int degatsIA = 0;

            if (actionJoueur == 1) //Si le joueur Attaque, on attaque selon le rôle qu'il a
            {
                if (role == "J")
                {
                    degatsIA -= aleatoire.Next(0, 3);
                    var audioFile = new NAudio.Wave.AudioFileReader("damage.wav"); //Son de dégats infligé
                    var outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    Thread.Sleep(100);
                }
                else
                {
                    degatsIA -= DommageParRole(role); //On inflige les dégats selon le rôle
                    var audioFile = new NAudio.Wave.AudioFileReader("damage.wav");
                    var outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    Thread.Sleep(100);
                }
                
                
            }
            if (actionIA == 1) //Si l'IA Attaque, on attaque selon le rôle qu'il a
            {
                if (roleordi == "J")
                {
                    degatsJoueur-= aleatoire.Next(0, 3);
                    var audioFile = new NAudio.Wave.AudioFileReader("damage.wav");
                    var outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    Thread.Sleep(100);
                }
                else
                {
                    degatsJoueur -= DommageParRole(roleordi);
                    var audioFile = new NAudio.Wave.AudioFileReader("damage.wav");
                    var outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    Thread.Sleep(100);
                }
            }



            if (actionJoueur == 2) //Si le joueur se défend
            {
                degatsJoueur = 0; //On met les dégats à 0
            }
            if (actionIA == 2) //Si l'IA se défend
            {
                degatsIA = 0;
            }



            if (actionJoueur == 3) //Si le joueur fait son coup spécial
            {
                isAttack = false;
                if (role == "J") //Coup spécial du Joker
                {
                    int dice = aleatoire.Next(1, 7); //On tire un dé

                    var audioFile = new NAudio.Wave.AudioFileReader("roll.mp3");
                    var outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    Thread.Sleep(500);

                    Affichage("Le dé à fait :"); Console.WriteLine(dice.ToString());
                    WriteDiceEffect(dice, true);
                    Thread.Sleep(2500);
                    if (dice == 1 || dice == 2) //Puis on fait une action selon le chiffre obtenu (voir README)
                    {
                        
                        degatsJoueur -= 10;
                    }
                    if (dice == 3)
                    {
                        degatsJoueur -= joueurVies / 2;
                    }
                    if (dice == 4)
                    {
                        
                        degatsIA -= ordiVies / 2;
                    }
                    if (dice == 5 || dice == 6)
                    {
                        degatsIA -= 10;
                    }
                }
                if (role == "H") //Coup spécial du Healer
                {
                    degatsJoueur += 2; //On Soigne
                    var audioFile = new NAudio.Wave.AudioFileReader("heal.wav");
                    var outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    Thread.Sleep(100);
                }
                if (role == "T") //Coup spécial du Tank
                {
                    degatsJoueur -= 1;
                    if (actionIA == 2) //On applique les dommages selon le rôle est l'action de l'adversaire
                    {
                        degatsIA -= 1;
                    }
                    else
                    {
                        degatsIA -= 2;
                    }
                }
                if (role == "D") //Coup spécial du Damager
                {
                    degatsIA += degatsJoueur; //On applique les dégats déjà subi par le joueur
                    if (actionIA == 3 & roleordi == "T")
                    {
                        degatsIA -= 2;
                    }
                }
            }

            if (actionIA == 3) //Si l'IA fait son coup spécial
            {
                isAttackOrdi = false;
                if (roleordi == "J")
                {
                    int dice = aleatoire.Next(1, 7);

                    var audioFile = new NAudio.Wave.AudioFileReader("roll.mp3");
                    var outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    Thread.Sleep(500);

                    Affichage("Le dé ennemi à fait :"); Console.WriteLine(dice.ToString());
                    WriteDiceEffect(dice, false);
                    Thread.Sleep(2500);
                    if (dice == 1 || dice == 2)
                    {
                        degatsIA -= 10;
                    }
                    if (dice == 3)
                    {
                        degatsIA -= ordiVies / 2;
                    }
                    if (dice == 4)
                    {
                        degatsJoueur -= joueurVies / 2;
                    }
                    if (dice == 5 || dice == 6)
                    {
                        degatsJoueur -= 10;
                    }
                }
                if (roleordi == "H")
                {
                    var audioFile = new NAudio.Wave.AudioFileReader("heal.wav");
                    var outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    Thread.Sleep(100);
                    degatsIA += 2;
                }
                if (roleordi == "T")
                {
                    degatsIA -= 1;
                    if (actionJoueur == 2)
                    {
                        degatsJoueur -= 1;
                    }
                    else
                    {
                        degatsJoueur -= 2;
                    }
                }
                if (roleordi == "D")
                {
                    degatsJoueur += degatsIA;
                }
            }

            return new Tuple<int, int>(degatsJoueur, degatsIA);

        }

        static int DommageParRole(string role)
        {
            //Fonction qui permet de savoir pour chaque rôle les dégats effectuer
            var charactersDM = new Dictionary<string, int>() { { "H", 1 }, { "T", 1 }, { "D", 2 } , {"J", 0 } };
            return charactersDM[role];
        }

        #region IA
        static string IaRandomChoice(List<string> options)
        {
            //Permet à l'IA de choisir une action alléatoire
            Random rand = new Random();
            return options[rand.Next(0, options.Count)];
        }
        static int ChoixIANormal(string role, string rolejoueur, int pvordi, int pvjoueur, ref bool isAttack, ref bool isAttackjoueur)
        {
            //Cette focntion est utilisé si l'IA est en "qualifiée", elle permet de faire de vrai choix, et non de l'aléatoire

            Random aleatoire = new Random();

            if (role != "J" && pvjoueur <= DommageParRole(role))
            {
                return 1;
            }

            else if (role == "D")
            {
                if (rolejoueur == "T" && aleatoire.Next(1, 4) == 1 && !isAttackjoueur)
                {
                    if (isAttack)
                    {
                        return 3;
                    }

                    return 2;
                }

                return 1;
            }

            else if (role == "T")
            {
                if (aleatoire.Next(1, 3) == 1 && isAttack)
                {
                    return 3;
                }

                return 1;
            }

            else if (role == "H")
            {
                if (pvjoueur <= 2 && isAttack)
                {
                    return 3;
                }

                else if (rolejoueur == "T" && isAttackjoueur)
                {
                    return 2;
                }

                return 1;
            }

            else if (role == "J" && isAttack)
            {
                return 3;
            }

            else if (isAttack)
            {
                return aleatoire.Next(1, 4);
            }

            else
            {
                return aleatoire.Next(1, 3);
            }
        }
        #endregion

        #region Affichage
        static void WriteDiceEffect(int diceroll, bool joueurornot)
        {
            //Fonction qui sert à écrire une phrase, indiquant au joueur quel effet de dé va être appliqué, selon le chiffre qu'il a obtenu

            var audioFile = new NAudio.Wave.AudioFileReader("nothing.mp3");
            var outputDevice = new NAudio.Wave.WaveOutEvent();
            if (joueurornot) //If qui permet de savoir si c'est le joueur ou l'ordi qui à tiré le dé
            {
                if (diceroll < 3) //Si c'est inférieur à 3
                {
                    audioFile = new NAudio.Wave.AudioFileReader("badroll.wav"); //Lecture de sons selon si l'effet est bon ou mauvais
                    outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    AffichageLine("Dommage, vous mourrez !"); 
                }
                if (diceroll == 3) // Si c'est égal à 3
                {
                    audioFile = new NAudio.Wave.AudioFileReader("badroll.wav");
                    outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    AffichageLine("Vous perdez la moitié de votre vie !");
                }
                if (diceroll == 4) // Si c'est égal à 4
                {
                    audioFile = new NAudio.Wave.AudioFileReader("goodroll.wav");
                    outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    AffichageLine("L'ennemi perd la moitié de sa vie !");
                }
                if (diceroll > 4) // Si c'est supérieur à 4
                {
                    audioFile = new NAudio.Wave.AudioFileReader("goodroll.wav");
                    outputDevice = new NAudio.Wave.WaveOutEvent();
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    AffichageLine("Bravo, l'ennemi meurt !");
                }
            }
            else
            {
                if (diceroll < 3)
                {
                    AffichageLine("Dommage pour lui, mais l'ennemi meurt !");
                }
                if (diceroll == 3)
                {
                    AffichageLine("Il perd la moitié de sa vie");
                }
                if (diceroll == 4)
                {
                    AffichageLine("Il vous a eu ! Vous perdez la moitié de votre vie !");
                }
                if (diceroll > 4)
                {
                    AffichageLine("Sa chance et comparable à votre nullité, donc vous mourrez !");
                }
            }
            Console.WriteLine();
        }

        static void Title()
        {
            //Fonction permettant l'affichage du Titre à chaque nettoyage de la console

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine();
            List<string> title = new List<string> { " █     █░ ▒█████   ██▀███  ▓█████▄      █████▒ ██▓  ▄████  ██░ ██ ▄▄▄█████▓▓█████  ██▀███  ▒███████▒", "▓█░ █ ░█░▒██▒  ██▒▓██ ▒ ██▒▒██▀ ██▌   ▓██   ▒ ▓██▒ ██▒ ▀█▒▓██░ ██▒▓  ██▒ ▓▒▓█   ▀ ▓██ ▒ ██▒▒ ▒ ▒ ▄▀░", "▒█░ █ ░█ ▒██░  ██▒▓██ ░▄█ ▒░██   █▌   ▒████ ░ ▒██▒▒██░▄▄▄░▒██▀▀██░▒ ▓██░ ▒░▒███   ▓██ ░▄█ ▒░ ▒ ▄▀▒░ ", "░█░ █ ░█ ▒██   ██░▒██▀▀█▄  ░▓█▄   ▌   ░▓█▒  ░ ░██░░▓█  ██▓░▓█ ░██ ░ ▓██▓ ░ ▒▓█  ▄ ▒██▀▀█▄    ▄▀▒   ░", "░░██▒██▓ ░ ████▓▒░░██▓ ▒██▒░▒████▓    ░▒█░    ░██░░▒▓███▀▒░▓█▒░██▓  ▒██▒ ░ ░▒████▒░██▓ ▒██▒▒███████▒", "░ ▓░▒ ▒  ░ ▒░▒░▒░ ░ ▒▓ ░▒▓░ ▒▒▓  ▒     ▒ ░    ░▓   ░▒   ▒  ▒ ░░▒░▒  ▒ ░░   ░░ ▒░ ░░ ▒▓ ░▒▓░░▒▒ ▓░▒░▒", "  ▒ ░ ░    ░ ▒ ▒░   ░▒ ░ ▒░ ░ ▒  ▒     ░       ▒ ░  ░   ░  ▒ ░▒░ ░    ░     ░ ░  ░  ░▒ ░ ▒░░░▒ ▒ ░ ▒", "  ░   ░  ░ ░ ░ ▒    ░░   ░  ░ ░  ░     ░ ░     ▒ ░░ ░   ░  ░  ░░ ░  ░         ░     ░░   ░ ░ ░ ░ ░ ░", "    ░        ░ ░     ░        ░                ░        ░  ░  ░  ░            ░  ░   ░       ░ ░    ", "                            ░                                                              ░        \n\n" };
            for (int i = 0; i < title.Count; i++)
            {
                Console.SetCursorPosition((Console.WindowWidth - title[i].Length) / 2, Console.CursorTop);
                Console.WriteLine(title[i]);
            }
            Console.ForegroundColor = ConsoleColor.White;

        }

        static void AfficherStats(int toppos, int index, Tuple<int, int> Currentpos)
        {   
            //Cette fonction affiche les stats de la classe actuellement surligné par le curseur lors de la selection
            //Entrées : toppos, qui permet d'afficher sur la bonne ligne; index, qui est l'index du choix actuellement surligné; Currentpos, qui sauvegarde la position du curseur vant d'entrée dans la fonction

            var charactersRoles = new Dictionary<int, string>() { { 1, "D" }, { 2, "H" }, { 3, "T" }, { 4, "J" } }; //Initialisation des variables nécessaire à l'affichage
            var PvRoles = new Dictionary<string, int>() { { "D", 3 }, { "H", 4 }, { "T", 5 } };
            var CapSpé = new Dictionary<int, List<string>>() { { 1, new List<string> { "Capacité spéciale : Inflige en retour les dégâts qui", "lui sont infligés durant ce tour. Les dégâts sont ", "quand même subis par le Damager." } }, { 2, new List<string> { "Capacité spéciale : Récupère 2 points de vie." } }, { 3, new List<string> { "Capacité spéciale : S'inflige 1 de dégats, mais en ", "inflige 2 à son adversaire. Si celui-ci se défend, ", "1 dégat lui sera tout de même infligé." } }, { 4, new List<string> { "Capacité spéciale : Il lance un dé, et un effet ", "s'applique selon le chiffre obtenu :", "  - 1 et 2 : Vous mourrez", "  - 3 : Vous perdez la moitié de votre vie", "  - 4 : L'ennemi perd la moitié de sa vie", "  - 5 et 6 : L'ennemi meurt" } } };

            //Nettoyage permetant un bon affichage des stats, avec repositionnement du curseur
            Console.SetCursorPosition(80, Console.CursorTop);
            Console.SetCursorPosition(80, Console.CursorTop - 1);
            Console.Write("                                                       ");
            Console.SetCursorPosition(80, Console.CursorTop);

            //Affichage de la vie selon la classe
            Console.Write("Vie : ");
            if (index != 4)
            {
                for (int i = 0; i < PvRoles[charactersRoles[index]]; i++)
                {
                    Console.Write("♥");
                }
            }
            else
            {
                Console.Write("?");
            }
            Console.Write("                                        ");

            //Affichage de l'attaque selon la classe
            Console.SetCursorPosition(80, Console.CursorTop + 1);
            Console.Write("Attaque : ");
            if (index != 4)
            {
                for (int i = 0; i < DommageParRole(charactersRoles[index]); i++)
                {
                    Console.Write("⚔");
                    Console.Write(" ");
                }
            }
            else
            {
                Console.Write("?");
            }
            Console.Write("                                        ");

            //Affichage des détails de la capacité spéciale selon la classe
            Console.SetCursorPosition(80, Console.CursorTop + 1);
            for (int j = 0; j < CapSpé[index].Count; j++)
            {
                Console.Write(CapSpé[index][j]);
                for (int k = 0; k < CapSpé[1][0].Length - CapSpé[index][j].Length; k++)
                {
                    Console.Write(" ");
                }
                Console.SetCursorPosition(80, Console.CursorTop + 1);


            }

            //Nettoyage des lignes restantes
            Console.Write("                                                    ");
            Console.SetCursorPosition(80, Console.CursorTop + 1);
            Console.Write("                                                    ");
            Console.SetCursorPosition(80, Console.CursorTop + 1);
            Console.Write("                                                    ");
            Console.SetCursorPosition(80, Console.CursorTop + 1);
            Console.Write("                                                    ");

            Console.SetCursorPosition(Currentpos.Item1, Currentpos.Item2); //Remise du curseur à sa position d'origine


        }

        static void ClearStats(int toppos, Tuple<int, int> Currentpos)
        {
            //Fonction complémentaire de "AfficherStats", qui sert à supprimé l'affichage des stats lorsqu'une classe à été choisi
            //Entrées : toppos, qui permet d'afficher sur la bonne ligne; Currentpos, qui sauvegarde la position du curseur vant d'entrée dans la fonction

            Console.SetCursorPosition(80, toppos); //On met le curseur au bon endroit
            for (int i = 0; i < 8; i++) //On print des espaces sur 8 lignes pour remplacer le text
            {
                Console.Write("                                                       ");
                Console.SetCursorPosition(80, Console.CursorTop);
            }
            Console.SetCursorPosition(Currentpos.Item1, Currentpos.Item2); //On remet le curseur au bon endroit
        }

        static void AffichageVie(string joueurRole, string ordiRole, int joueurVies, int ordiVies)
        {
            //Fonction permettant l'affichage de la barre de vie

            string BarresVie = ""; //Barre vie permet de mettre la barre de vie au milieu de l'écran
            Console.Write("                                  ");
            Console.Write("Joueur " + joueurRole + " : ");
            BarresVie += "Joueur " + joueurRole + " : ";
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < joueurVies; i++) //Affichage des pv restant du joueur
            {
                Console.Write("♥");
                BarresVie += "♥";
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;

            //Affichage des pv perdu du joueur, selon son rôle
            if (joueurRole == "H")
            {
                for (int i = 0; i < 6 - joueurVies; i++)
                {
                    Console.Write("♥");
                    BarresVie += "♥";
                }
            }
            else if (joueurRole == "D")
            {
                for (int i = 0; i < 3 - joueurVies; i++)
                {
                    Console.Write("♥");
                    BarresVie += "♥";
                }
            }
            else if (joueurRole == "T")
            {
                for (int i = 0; i < 5 - joueurVies; i++)
                {
                    Console.Write("♥");
                    BarresVie += "♥";
                }
            }
            Console.ForegroundColor = ConsoleColor.White;


            Console.Write("                                      ");
            BarresVie += "                                      ";

            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < ordiVies; i++) //Pareil, mais pour l'IA
            {
                Console.Write("♥");
                BarresVie += "♥";
            }
            Console.ForegroundColor = ConsoleColor.DarkRed;
            if (ordiRole == "H")
            {
                for (int i = 0; i < 6 - ordiVies; i++)
                {
                    Console.Write("♥");
                    BarresVie += "♥";
                }
            }
            else if (ordiRole == "D")
            {
                for (int i = 0; i < 3 - ordiVies; i++)
                {
                    Console.Write("♥");
                    BarresVie += "♥";
                }
            }
            else if (ordiRole == "T")
            {
                for (int i = 0; i < 5 - ordiVies; i++)
                {
                    Console.Write("♥");
                    BarresVie += "♥";
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" : Ordinateur " + ordiRole);
            BarresVie += " : Ordinateur " + ordiRole;
        }

        static void AffichageLine(string text)
        {
            //Permet l'affichage au milieu de l'écran, en plus de faire un retour à la ligne
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop);
            Console.WriteLine(text);
        }

        static void Affichage(string text)
        {
            //Permet l'affichage au milieu de l'écran
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop);
            Console.Write(text);
        }
        #endregion

        #region  PleinEcran
        //Permet l'affichage en plein écran
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int MAXIMIZE = 3;
        #endregion

        #region Police
        //Permet de grossir la police d'écriture (et de la changer accessoirement)
        private const int FixedWidthTrueType = 54;
        private const int StandardOutputHandle = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);


        private static readonly IntPtr ConsoleOutputHandle = GetStdHandle(StandardOutputHandle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FontInfo
        {
            internal int cbSize;
            internal int FontIndex;
            internal short FontWidth;
            public short FontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.wc, SizeConst = 32)]
            public string FontName;
        }

        public static FontInfo[] SetCurrentFont(string font, short fontSize = 0)
        {
            FontInfo before = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>()
            };

            if (GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before))
            {

                FontInfo set = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>(),
                    FontIndex = 0,
                    FontFamily = FixedWidthTrueType,
                    FontName = font,
                    FontWeight = 400,
                    FontSize = fontSize > 0 ? fontSize : before.FontSize
                };

                // Get some settings from current font.
                if (!SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set))
                {
                    var ex = Marshal.GetLastWin32Error();
                    Console.WriteLine("Set error " + ex);
                    throw new System.ComponentModel.Win32Exception(ex);
                }

                FontInfo after = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>()
                };
                GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);

                return new[] { before, set, after };
            }
            else
            {
                var er = Marshal.GetLastWin32Error();
                Console.WriteLine("Get error " + er);
                throw new System.ComponentModel.Win32Exception(er);
            }
        }
        #endregion
    }
}