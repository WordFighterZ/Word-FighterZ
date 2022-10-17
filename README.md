# Word FighterZ

Word FighterZ est un jeu de combat tour par tour, fonctionnant entièrement via la console grâce au langage C#, tout en vous permettant de faire des choix à l'aide des flèches de votre clavier, de profiter d'une musique... dans le but de vaincre votre adversaire !

‎ 
## Comment lancer le jeu

Rien de plus simple, il vous suffit de lancer l'application "Word FighterZ" présente dans l'archive.

‎ 
## Comment jouer au jeu

Vous aurez dans le jeux une succession de choix à faire. Vous pourrez alors utiliser les flèches pour choisir, puis Entrée pour confirmer votre choix.

![AltText](/repository/GIF/CursorMove.gif?raw=true)

Vous avez plusieurs actions possible :

- Attaque : Elle inflige à l'adversaire 1 à 2 de dégats suivant la classe que vous avez choisi. *Voir Explication des rôles pour plus de détails*

- Défense : Annule tout les dégats subit pendant ce tour, sauf un cas spécifique qui est la capacité spécial du Tank.

- Capacité spéciale : Une attaque spéciale unique à chaque rôle, utilisable seulement 1 fois par partie, alors utilisez là avec parcimonie !

(*Voir Explication des classes pour plus de détails*)

‎ 
## Explication des différentes classes

### - Damager 
![AltText](/repository/GIF/Damager.png?raw=true) 

>Classe infligeant beaucoup de dégats, mais n'est pas très résistant.
>
>Vie : ♥♥♥
>
>Attaque : ⚔⚔
>
>Capacité spéciale : Inflige en retour les dégâts qui
lui sont infligés durant ce tour. Les dégâts sont quand même
subis par le Damager.

### - Tank 
![AltText](/repository/GIF/Tank.png?raw=true) 

>Classe n'infligeant pas beaucoup de dégats, mais ayant beaucoup de vie.
>
>Vie : ♥♥♥♥♥
>
>Attaque : ⚔
>
>Capacité spéciale : S'inflige 1 de dégats, mais en inflige 2 à son adversaire. Si celui-ci se défend, 1 dégat lui sera tout de même infligé.

### - Healer 
![AltText](/repository/GIF/Healer.png?raw=true) 

>Classe n'infligeant pas beaucoup de dégats, Mais pouvant se soigner.
>
>Vie : ♥♥♥♥
>
>Attaque : ⚔
>
>Capacité spéciale : Récupère 2 points de vie.

### - Joker 
![AltText](/repository/GIF/Joker.png?raw=true) 

>Une classe bien mystérieuse, mais finalement simple à comprendre : tout est aléatoire, de ses points de vie, dégats, à son coup spécial.
>
>Vie : ??? (de 1 à 4)
>
>Attaque : ??? (de 0 à 2, change à chaque tour)
>
>Capacité spéciale : Il lance un dé, et un effet s'applique selon le chiffre obtenu :
> - 1 et 2 : Vous mourrez
> - 3 : Vous perdez la moitié de votre vie
> - 4 : L'ennemi perd la moitié de sa vie
> - 5 et 6 : L'ennemi meurt

‎ 

‎ 
# Word FighterZ IA vs IA

Il y a dans l'archive un second executable. Il s'agit de la version ordinateur vs ordinateur du jeu, permettant de tester des centaines de parties en quelques secondes.

## Fonctionnement

La seul que vous avez à faire et choisir la classe des 2 ordinateurs, puis observez les matches défiler à la vitesse de la lumière !

‎ 

‎ 
# Sources et crédits

Bruitage du lancement de dé : https://www.youtube.com/watch?v=KY4hU1BYWEc&ab_channel=NyxAssets

Musique : Joshua McLean - Mountain Trials  https://www.youtube.com/watch?v=L_OYo2RS8iU&t=15s&ab_channel=FreeMusic

Site de création de bruitage : https://sfxr.me/

‎ 

‎
## Par Ryan BLOT, Balthazar BRILLAULT D. et Noa HUSSENET
