using procrastinate.Services;

namespace procrastinate.Pages;

public partial class TasksPage : ContentPage
{
    private readonly StatsService _statsService;

    public TasksPage(StatsService statsService)
    {
        InitializeComponent();
        _statsService = statsService;
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SettingsPage));
    }

    private void OnTaskCardTapped(object? sender, TappedEventArgs e)
    {
        TaskCheckBox.IsChecked = !TaskCheckBox.IsChecked;
    }

    private async void OnTaskChecked(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            _statsService.IncrementBreaksTaken();
            MotivationLabel.Text = AppStrings.GetString("Congratulations");
            await Task.Delay(2000);
            TaskCheckBox.IsChecked = false;
            MotivationLabel.Text = AppStrings.GetString("NeedAnotherBreak");
        }
    }

    private async void OnAddTaskClicked(object? sender, EventArgs e)
    {
        // Refresh zalgo randomness on button click
        AppStrings.Refresh();
        
        _statsService.IncrementTasksAvoided();
        var excuses = GetLocalizedExcuses();
        var excuse = excuses[Random.Shared.Next(excuses.Length)];
        
        // Apply Zalgo if enabled
        var displayExcuse = AppStrings.IsZalgoMode ? AppStrings.Zalgoify(excuse) : excuse;
        await DisplayAlertAsync("❌", displayExcuse, "OK");
    }

    private string[] GetLocalizedExcuses()
    {
        return AppStrings.CurrentLanguage switch
        {
            "fr" => [
                "Oups! La liste est pleine. Réessayez demain!", 
                "Erreur 404: Productivité introuvable.", 
                "Tâche rejetée: Vous méritez une pause!", 
                "Le serveur fait la sieste. Comme vous devriez!", 
                "Productivité maximale atteinte! (1 tâche = maximum)",
                "Votre quota de tâches est épuisé. Revenez l'année prochaine!",
                "Impossible d'ajouter: Le bonheur est plus important.",
                "La liste des tâches est en grève. Solidarité!",
                "Votre cerveau a besoin de repos. C'est scientifique!",
                "Erreur système: Trop de motivation détectée."
            ],
            "es" => [
                "¡Ups! La lista está llena. ¡Inténtalo mañana!", 
                "Error 404: Productividad no encontrada.", 
                "¡Tarea rechazada: Te mereces un descanso!", 
                "El servidor está durmiendo. ¡Como tú deberías!", 
                "¡Productividad máxima alcanzada! (1 tarea = máximo)",
                "Tu cuota de tareas está agotada. ¡Vuelve el próximo año!",
                "Imposible agregar: La felicidad es más importante.",
                "La lista de tareas está en huelga. ¡Solidaridad!",
                "Tu cerebro necesita descanso. ¡Es científico!",
                "Error del sistema: Demasiada motivación detectada."
            ],
            "pt" => [
                "Ops! A lista está cheia. Tente amanhã!", 
                "Erro 404: Produtividade não encontrada.", 
                "Tarefa rejeitada: Você merece uma pausa!", 
                "O servidor está dormindo. Como você deveria!", 
                "Produtividade máxima atingida! (1 tarefa = máximo)",
                "Sua cota de tarefas acabou. Volte no próximo ano!",
                "Impossível adicionar: A felicidade é mais importante.",
                "A lista de tarefas está em greve. Solidariedade!",
                "Seu cérebro precisa de descanso. É científico!",
                "Erro do sistema: Muita motivação detectada."
            ],
            "nl" => [
                "Oeps! De lijst is vol. Probeer morgen!", 
                "Fout 404: Productiviteit niet gevonden.", 
                "Taak afgewezen: Je verdient een pauze!", 
                "De server slaapt. Net als jij zou moeten!", 
                "Maximale productiviteit bereikt! (1 taak = maximum)",
                "Je takenlimiet is bereikt. Kom volgend jaar terug!",
                "Kan niet toevoegen: Geluk is belangrijker.",
                "De takenlijst staakt. Solidariteit!",
                "Je brein heeft rust nodig. Het is wetenschap!",
                "Systeemfout: Te veel motivatie gedetecteerd."
            ],
            "cs" => [
                "Jejda! Seznam je plný. Zkuste zítra!", 
                "Chyba 404: Produktivita nenalezena.", 
                "Úkol odmítnut: Zasloužíte si pauzu!", 
                "Server spí. Jako byste měli vy!", 
                "Maximální produktivita dosažena! (1 úkol = maximum)",
                "Vaše kvóta úkolů je vyčerpána. Vraťte se příští rok!",
                "Nelze přidat: Štěstí je důležitější.",
                "Seznam úkolů stávkuje. Solidarita!",
                "Váš mozek potřebuje odpočinek. Je to vědecké!",
                "Systémová chyba: Detekováno příliš mnoho motivace."
            ],
            "uk" => [
                "Ой! Список повний. Спробуйте завтра!", 
                "Помилка 404: Продуктивність не знайдена.", 
                "Завдання відхилено: Ви заслуговуєте на перерву!", 
                "Сервер дрімає. Як і ви повинні!", 
                "Досягнуто максимальну продуктивність! (1 завдання = максимум)",
                "Ваша квота завдань вичерпана. Повертайтеся наступного року!",
                "Неможливо додати: Щастя важливіше.",
                "Список завдань страйкує. Солідарність!",
                "Вашому мозку потрібен відпочинок. Це наука!",
                "Системна помилка: Виявлено занадто багато мотивації.",
                "Переповнення буфера завдань. Будь ласка, прокрастинуйте.",
                "Додавання завдань тимчасово вимкнено для вашого блага.",
                "Фея завдань у відпустці. Спробуйте ніколи!"
            ],
            _ => [
                "Oops! The task list is full. Try again tomorrow!", 
                "Error 404: Productivity not found.", 
                "Task rejected: You deserve a break instead!", 
                "Server is napping. Just like you should be!", 
                "Maximum productivity reached! (1 task = maximum)",
                "Your task quota is exhausted. Come back next year!",
                "Cannot add: Happiness is more important.",
                "The task list is on strike. Solidarity!",
                "Your brain needs rest. It's science!",
                "System error: Too much motivation detected.",
                "Task buffer overflow. Please procrastinate.",
                "Adding tasks is temporarily disabled for your wellbeing.",
                "The task fairy is on vacation. Try again never!"
            ]
        };
    }
}
