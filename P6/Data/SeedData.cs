using Microsoft.EntityFrameworkCore;
using P6.Models;
using OperatingSystem = P6.Models.OperatingSystem;
using Version = P6.Models.Version;

namespace P6.Data;

/// <summary>
/// Test data: products, versions and compatibilities from the "Tableau des produits et versions"
/// document, and the 25 sample tickets. Run at startup (see Program.cs); does nothing if the
/// database already contains a product.
/// </summary>
public static class SeedData
{
    public const int StatusInProgressId = 1;
    public const int StatusResolvedId = 2;

    /// <summary>
    /// Statuses are reference values created with the schema (HasData): the CHECK constraint
    /// on Ticket relies on their fixed ids.
    /// </summary>
    public static void SeedStatuses(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Status>().HasData(
            new Status { Id = StatusInProgressId, Label = "En cours" },
            new Status { Id = StatusResolvedId, Label = "Résolu" });
    }

    /// <summary>A database that already has a product has already been seeded.</summary>
    public static async Task SeedAsync(NexaWorksContext db)
    {
        if (await db.Products.AnyAsync())
        {
            return;
        }

        var linux         = new OperatingSystem { Name = "Linux" };
        var macOs         = new OperatingSystem { Name = "MacOS" };
        var windows       = new OperatingSystem { Name = "Windows" };
        var android       = new OperatingSystem { Name = "Android" };
        var iOs           = new OperatingSystem { Name = "iOS" };
        var windowsMobile = new OperatingSystem { Name = "Windows Mobile" };

        var traderEnHerbe             = new Product { Name = "Trader en Herbe" };
        var maitreDesInvestissements  = new Product { Name = "Maître des Investissements" };
        var planificateurEntrainement = new Product { Name = "Planificateur d’Entraînement" };
        var planificateurAnxiete      = new Product { Name = "Planificateur d’Anxiété Sociale" };

        // Compatibility table: one VersionOperatingSystem per "x" of the document.
        var trader10       = NewVersion(traderEnHerbe,             "1.0", linux, windows);
        var trader11       = NewVersion(traderEnHerbe,             "1.1", linux, macOs, windows);
        var trader12       = NewVersion(traderEnHerbe,             "1.2", linux, macOs, windows, android, iOs, windowsMobile);
        var trader13       = NewVersion(traderEnHerbe,             "1.3", macOs, windows, android, iOs);
        var maitre10       = NewVersion(maitreDesInvestissements,  "1.0", macOs, iOs);
        var maitre20       = NewVersion(maitreDesInvestissements,  "2.0", macOs, android, iOs);
        var maitre21       = NewVersion(maitreDesInvestissements,  "2.1", macOs, windows, android, iOs);
        var entrainement10 = NewVersion(planificateurEntrainement, "1.0", linux, macOs);
        var entrainement11 = NewVersion(planificateurEntrainement, "1.1", linux, macOs, windows, android, iOs, windowsMobile);
        var entrainement20 = NewVersion(planificateurEntrainement, "2.0", macOs, windows, android, iOs);
        var anxiete10      = NewVersion(planificateurAnxiete,      "1.0", macOs, windows, android, iOs);
        var anxiete11      = NewVersion(planificateurAnxiete,      "1.1", macOs, windows, android, iOs);

        db.Products.AddRange(traderEnHerbe, maitreDesInvestissements, planificateurEntrainement, planificateurAnxiete);

        db.Tickets.AddRange(
            // Ticket 1: Trader en Herbe 1.0 - Linux
            Resolved(trader10[linux], new(2022, 3, 14), new(2022, 3, 29),
                "L’utilisateur indique que, depuis la mise à jour de sa distribution vers Ubuntu 22.04, les graphiques de cours n’affichent plus qu’une zone blanche. Les données chiffrées (prix, volumes) s’affichent correctement.",
                "La nouvelle version d’Ubuntu utilise Wayland par défaut, incompatible avec le moteur de rendu graphique de l’application. Contournement communiqué : ouvrir une session « Ubuntu sur Xorg ». Demande envoyée à l’équipe de développement pour basculer automatiquement sur le rendu logiciel lorsque Wayland est détecté."),

            // Ticket 2: Trader en Herbe 1.0 - Windows
            Resolved(trader10[windows], new(2022, 5, 2), new(2022, 5, 6),
                "L’utilisateur ne parvient plus à se connecter : le message « Erreur d’authentification » apparaît alors que son mot de passe est correct et fonctionne sur un autre ordinateur.",
                "L’horloge du PC de l’utilisateur avait 12 minutes de retard, ce qui rendait le jeton d’authentification invalide. Activation de la synchronisation automatique de l’heure dans Windows. Demande envoyée à l’équipe de développement pour afficher un message d’erreur explicite en cas de décalage horaire."),

            // Ticket 3: Trader en Herbe 1.1 - MacOS
            InProgress(trader11[macOs], new(2022, 9, 12),
                "Après la sortie de veille du Mac, les cours en temps réel ne se mettent plus à jour. L’utilisateur doit quitter puis relancer l’application pour que les prix redeviennent actualisés."),

            // Ticket 4: Trader en Herbe 1.1 - Linux
            Resolved(trader11[linux], new(2022, 10, 3), new(2022, 10, 20),
                "Lorsque l’utilisateur exporte l’historique de ses transactions en CSV et l’ouvre dans LibreOffice Calc, les montants à virgule sont coupés en deux et décalent toutes les colonnes suivantes.",
                "Le fichier utilisait la virgule comme séparateur de colonnes, alors que les montants sont formatés avec une virgule décimale (paramètres régionaux français). L’export utilise désormais le point-virgule comme séparateur. Correctif livré et validé avec l’utilisateur."),

            // Ticket 5: Trader en Herbe 1.2 - Android
            Resolved(trader12[android], new(2023, 4, 11), new(2023, 5, 2),
                "L’utilisateur a configuré une alerte lorsque l’action Total passe sous 55 €, mais ne reçoit aucune notification si l’application n’est pas ouverte.",
                "L’optimisation de batterie du téléphone (Samsung) fermait le service de l’application en arrière-plan. L’utilisateur a exclu l’application de l’optimisation de batterie et reçoit désormais les alertes. Demande envoyée à l’équipe de développement pour passer par des notifications push serveur plutôt que par un service local."),

            // Ticket 6: Trader en Herbe 1.2 - Windows Mobile
            InProgress(trader12[windowsMobile], new(2023, 6, 19),
                "Sur un Lumia 950, l’application affiche un écran noir pendant quelques secondes au lancement, puis se ferme et renvoie l’utilisateur à l’écran d’accueil du téléphone."),

            // Ticket 7: Trader en Herbe 1.3 - iOS
            Resolved(trader13[iOs], new(2024, 1, 8), new(2024, 1, 22),
                "L’utilisateur doit valider Face ID à chaque changement d’onglet dans l’application (Portefeuille, Marché, Historique), ce qui rend la navigation très pénible.",
                "Le délai d’expiration de la session biométrique était à 0 seconde au lieu de 5 minutes suite à une erreur de configuration dans la version 1.3. Le correctif a été publié sur l’App Store et l’utilisateur a confirmé que le problème a disparu."),

            // Ticket 8: Trader en Herbe 1.3 - Windows
            InProgress(trader13[windows], new(2024, 2, 15),
                "L’utilisateur a choisi l’euro comme devise dans les paramètres, mais l’écran de résumé du portefeuille affiche toujours le solde total en dollars. Les autres écrans affichent bien des euros."),

            // Ticket 9: Maître des Investissements 1.0 - MacOS
            Resolved(maitre10[macOs], new(2022, 2, 7), new(2022, 2, 18),
                "L’import du relevé bancaire au format OFX échoue systématiquement avec le message « Format de fichier non reconnu ».",
                "Le fichier fourni par la banque était encodé en ISO-8859-1 et contenait des caractères accentués, alors que l’application n’acceptait que l’UTF-8. Contournement : réenregistrer le fichier en UTF-8. Demande envoyée à l’équipe de développement pour détecter automatiquement l’encodage à l’import."),

            // Ticket 10: Maître des Investissements 1.0 - iOS
            Resolved(maitre10[iOs], new(2022, 4, 25), new(2022, 5, 10),
                "Lorsque le mode sombre d’iOS est activé, les libellés du graphique de répartition du portefeuille sont écrits en noir sur fond noir et sont illisibles.",
                "Les couleurs du graphique étaient fixées en dur au lieu de suivre le thème du système. Le graphique utilise désormais les couleurs du thème actif. Correctif publié et confirmé par l’utilisateur."),

            // Ticket 11: Maître des Investissements 2.0 - Android
            InProgress(maitre20[android], new(2023, 3, 6),
                "Le rendement annualisé affiché pour une position achetée il y a 10 jours est de +4 500 %, alors que la position n’a gagné que 3 %."),

            // Ticket 12: Maître des Investissements 2.0 - MacOS
            Resolved(maitre20[macOs], new(2023, 3, 20), new(2023, 4, 3),
                "Lorsque l’application reste ouverte en arrière-plan, le ventilateur du Mac tourne en permanence et le Moniteur d’activité indique que l’application utilise 100 % d’un cœur du processeur.",
                "La boucle d’actualisation des cours continuait de tourner à pleine vitesse même lorsque la fenêtre n’était pas visible. Envoi d’une demande à l’équipe de développement, qui a suspendu les actualisations quand l’application est en arrière-plan. Correctif livré."),

            // Ticket 13: Maître des Investissements 2.1 - Windows
            InProgress(maitre21[windows], new(2023, 11, 14),
                "Après la saisie d’un dividende reçu, le total des revenus de l’année n’est pas mis à jour. Le bon montant n’apparaît qu’après un redémarrage de l’application."),

            // Ticket 14: Maître des Investissements 2.1 - iOS
            Resolved(maitre21[iOs], new(2024, 3, 4), new(2024, 3, 8),
                "L’utilisateur a demandé la réinitialisation de son mot de passe à plusieurs reprises mais ne reçoit jamais l’e-mail.",
                "Les e-mails étaient classés en courrier indésirable par la messagerie iCloud de l’utilisateur. L’utilisateur a ajouté l’expéditeur à ses contacts et a pu réinitialiser son mot de passe. Demande envoyée à l’équipe infrastructure pour vérifier la configuration SPF/DKIM du domaine d’envoi."),

            // Ticket 15: Planificateur d’Entraînement 1.0 - Linux
            Resolved(entrainement10[linux], new(2022, 1, 17), new(2022, 1, 31),
                "Le minuteur de repos entre deux séries s’affiche correctement mais aucun son n’est émis à la fin du décompte.",
                "L’application dépend de PulseAudio, alors que la distribution de l’utilisateur (Fedora) utilise PipeWire sans la couche de compatibilité. Installation du paquet pipewire-pulse : le son fonctionne. Demande envoyée à l’équipe de développement pour déclarer cette dépendance dans le paquet d’installation."),

            // Ticket 16: Planificateur d’Entraînement 1.0 - MacOS
            InProgress(entrainement10[macOs], new(2022, 3, 28),
                "Les séances programmées le dimanche apparaissent le lundi dans la vue hebdomadaire du calendrier. L’utilisateur a configuré son Mac avec une semaine commençant le dimanche."),

            // Ticket 17: Planificateur d’Entraînement 1.1 - Windows Mobile
            Resolved(entrainement11[windowsMobile], new(2022, 8, 8), new(2022, 8, 30),
                "La synchronisation avec le bracelet connecté de l’utilisateur échoue systématiquement avec le message « Appareil introuvable », alors que le Bluetooth est activé.",
                "Le bracelet de l’utilisateur n’implémente pas le profil Bluetooth Low Energy requis par l’application. L’utilisateur a été informé que son modèle n’est pas compatible. La liste des appareils compatibles a été ajoutée à la FAQ."),

            // Ticket 18: Planificateur d’Entraînement 1.1 - Android
            Resolved(entrainement11[android], new(2022, 10, 10), new(2022, 10, 14),
                "L’utilisateur indique que la distance de ses courses est doublée : une sortie de 5 km est enregistrée comme 10 km.",
                "L’utilisateur démarrait une séance depuis le widget de l’écran d’accueil puis une seconde depuis l’application : deux enregistrements GPS tournaient en parallèle et étaient additionnés. Explication donnée à l’utilisateur. Demande envoyée à l’équipe de développement pour empêcher le démarrage d’une seconde séance simultanée."),

            // Ticket 19: Planificateur d’Entraînement 1.1 - Linux
            InProgress(entrainement11[linux], new(2022, 11, 21),
                "Sur un écran 4K avec une mise à l’échelle de 200 %, la fenêtre de l’application s’affiche en tout petit et le texte est illisible."),

            // Ticket 20: Planificateur d’Entraînement 2.0 - iOS
            Resolved(entrainement20[iOs], new(2023, 7, 3), new(2023, 7, 17),
                "Les séances enregistrées par l’Apple Watch de l’utilisateur ne sont pas importées depuis l’app Santé.",
                "L’utilisateur avait refusé l’accès à Apple Santé lors du premier lancement. Il a été guidé pour réactiver l’autorisation dans Réglages > Confidentialité > Santé, et l’import fonctionne. Demande envoyée à l’équipe de développement pour afficher un écran d’explication lorsque l’autorisation est refusée."),

            // Ticket 21: Planificateur d’Entraînement 2.0 - Windows
            InProgress(entrainement20[windows], new(2023, 9, 25),
                "Lors de l’export en PDF du programme mensuel au format A4, la dernière colonne (dimanche) est coupée et n’apparaît pas sur la page."),

            // Ticket 22: Planificateur d’Anxiété Sociale 1.0 - Android
            Resolved(anxiete10[android], new(2023, 1, 16), new(2023, 2, 1),
                "L’utilisateur a programmé un rappel quotidien d’exercice de respiration à 9 h, mais reçoit la notification à 3 h du matin.",
                "L’utilisateur s’était inscrit à Montréal puis avait déménagé à Paris : l’heure du rappel était calculée avec le fuseau horaire enregistré à l’inscription. Envoi d’une demande à l’équipe de développement, qui utilise désormais le fuseau horaire de l’appareil. Correctif livré et rappel reprogrammé avec l’utilisateur."),

            // Ticket 23: Planificateur d’Anxiété Sociale 1.0 - Windows
            InProgress(anxiete10[windows], new(2023, 2, 27),
                "L’utilisateur perd le texte en cours de saisie dans son journal des émotions lorsqu’il passe à une autre fenêtre pendant quelques minutes avant de revenir dans l’application."),

            // Ticket 24: Planificateur d’Anxiété Sociale 1.1 - MacOS
            Resolved(anxiete11[macOs], new(2023, 10, 9), new(2023, 10, 12),
                "Après avoir désinstallé puis réinstallé l’application, l’utilisateur ne retrouve plus aucune de ses entrées de journal des six derniers mois.",
                "Les données étaient stockées uniquement en local car la sauvegarde cloud n’avait pas été activée. Les entrées ont été restaurées à partir de la sauvegarde Time Machine de l’utilisateur. Demande envoyée à l’équipe de développement pour proposer l’activation de la sauvegarde cloud dès la première utilisation."),

            // Ticket 25: Planificateur d’Anxiété Sociale 1.1 - iOS
            InProgress(anxiete11[iOs], new(2024, 5, 13),
                "La lecture des méditations guidées s’interrompt dès que l’écran de l’iPhone se verrouille, alors que l’utilisateur les écoute avec des écouteurs."));

        await db.SaveChangesAsync();
    }

    /// <summary>A product version and its supported operating systems, indexed by OS.</summary>
    private static Dictionary<OperatingSystem, VersionOperatingSystem> NewVersion(
        Product product, string number, params OperatingSystem[] operatingSystems)
    {
        var version = new Version { Number = number, Product = product };
        product.Versions.Add(version);

        var compatibilities = new Dictionary<OperatingSystem, VersionOperatingSystem>();
        foreach (var operatingSystem in operatingSystems)
        {
            var compatibility = new VersionOperatingSystem { Version = version, OperatingSystem = operatingSystem };
            version.VersionOperatingSystems.Add(compatibility);
            compatibilities[operatingSystem] = compatibility;
        }
        return compatibilities;
    }

    private static Ticket InProgress(VersionOperatingSystem versionOperatingSystem, DateOnly createdAt, string problem) =>
        new()
        {
            VersionOperatingSystem = versionOperatingSystem,
            StatusId = StatusInProgressId,
            CreatedAt = createdAt,
            Problem = problem
        };

    private static Ticket Resolved(
        VersionOperatingSystem versionOperatingSystem, DateOnly createdAt, DateOnly resolvedAt, string problem, string resolution) =>
        new()
        {
            VersionOperatingSystem = versionOperatingSystem,
            StatusId = StatusResolvedId,
            CreatedAt = createdAt,
            ResolvedAt = resolvedAt,
            Problem = problem,
            Resolution = resolution
        };
}
