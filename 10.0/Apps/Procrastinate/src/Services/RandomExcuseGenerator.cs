using System.Diagnostics;
using procrastinate.Services;

namespace procrastinate.Services;

public class RandomExcuseGenerator : IExcuseGenerator
{
    public string Name => "Random";
    public bool IsAvailable => true;

    public Task<ExcuseResult> GenerateExcuseAsync(string language)
    {
        var stopwatch = Stopwatch.StartNew();
        
        var (starters, middles, endings) = GetLocalizedExcuseParts(language);
        var starter = starters[Random.Shared.Next(starters.Length)];
        var middle = middles[Random.Shared.Next(middles.Length)];
        var ending = endings[Random.Shared.Next(endings.Length)];

        var excuse = $"{starter} {middle} {ending}";
        stopwatch.Stop();
        
        return Task.FromResult(new ExcuseResult(excuse, Name, stopwatch.Elapsed));
    }

    private static (string[], string[], string[]) GetLocalizedExcuseParts(string language)
    {
        return language switch
        {
            "fr" => (
                [
                    "Je ne peux pas parce que", "Désolé, mais", "J'aurais bien voulu, mais", 
                    "Malheureusement,", "J'ai essayé, mais", "C'est impossible car",
                    "Je dois refuser parce que", "Pas cette fois car", "J'adorerais, mais"
                ],
                [
                    "mon poisson rouge", "Mercure est en rétrograde et", "mon horoscope a dit que",
                    "un inconnu mystérieux", "mon WiFi", "mes plantes", "le chat du voisin",
                    "mon thérapeute a mentionné que", "mon coach de vie a insisté que",
                    "mon médecin m'a dit que", "mon intuition me dit que", "ma grand-mère m'a averti que",
                    "un rêve m'a révélé que", "les étoiles indiquent que", "mon karma suggère que"
                ],
                [
                    "a besoin de moi pour un soutien émotionnel.", "m'a spécifiquement mis en garde.",
                    "me juge trop en ce moment.", "a supprimé toute ma motivation.",
                    "m'a dit de faire une sieste.", "nécessite ma présence immédiate.",
                    "a prédit une catastrophe si je le fais.", "demande que je reste à la maison.",
                    "a mangé mes clés de voiture.", "provoque des allergies temporaires.",
                    "interfère avec mon aura.", "a créé un conflit d'horaire cosmique."
                ]
            ),
            "es" => (
                [
                    "No puedo porque", "Lo siento, pero", "Me encantaría, pero",
                    "Desafortunadamente,", "Lo intenté, pero", "Es imposible porque",
                    "Debo rechazar porque", "No esta vez porque", "Quisiera, pero"
                ],
                [
                    "mi pez dorado", "Mercurio está retrógrado y", "mi horóscopo dijo que",
                    "un extraño misterioso", "mi WiFi", "mis plantas", "el gato del vecino",
                    "mi terapeuta mencionó que", "mi coach de vida insistió que",
                    "mi médico me dijo que", "mi intuición me dice que", "mi abuela me advirtió que",
                    "un sueño me reveló que", "las estrellas indican que", "mi karma sugiere que"
                ],
                [
                    "me necesita para apoyo emocional.", "me advirtió específicamente.",
                    "me está juzgando demasiado.", "borró toda mi motivación.",
                    "me dijo que tomara una siesta.", "requiere mi presencia inmediata.",
                    "predijo una catástrofe si lo hago.", "exige que me quede en casa.",
                    "se comió mis llaves del carro.", "causa alergias temporales.",
                    "interfiere con mi aura.", "ha creado un conflicto cósmico de horario."
                ]
            ),
            "pt" => (
                [
                    "Não posso porque", "Desculpe, mas", "Eu gostaria, mas",
                    "Infelizmente,", "Eu tentei, mas", "É impossível porque",
                    "Preciso recusar porque", "Não desta vez porque", "Adoraria, mas"
                ],
                [
                    "meu peixinho dourado", "Mercúrio está retrógrado e", "meu horóscopo disse que",
                    "um estranho misterioso", "meu WiFi", "minhas plantas", "o gato do vizinho",
                    "meu terapeuta mencionou que", "meu coach de vida insistiu que",
                    "meu médico me disse que", "minha intuição me diz que", "minha avó me avisou que",
                    "um sonho me revelou que", "as estrelas indicam que", "meu karma sugere que"
                ],
                [
                    "precisa de mim para apoio emocional.", "me avisou especificamente.",
                    "está me julgando demais.", "deletou toda minha motivação.",
                    "me disse para tirar uma soneca.", "requer minha presença imediata.",
                    "previu uma catástrofe se eu fizer isso.", "exige que eu fique em casa.",
                    "comeu minhas chaves do carro.", "causa alergias temporárias.",
                    "interfere com minha aura.", "criou um conflito cósmico de horário."
                ]
            ),
            "nl" => (
                [
                    "Ik kan niet omdat", "Sorry, maar", "Ik zou wel willen, maar",
                    "Helaas,", "Ik probeerde, maar", "Het is onmogelijk want",
                    "Ik moet weigeren want", "Niet deze keer want", "Ik zou graag, maar"
                ],
                [
                    "mijn goudvis", "Mercurius is retrograde en", "mijn horoscoop zei dat",
                    "een mysterieuze vreemdeling", "mijn WiFi", "mijn planten", "de kat van de buren",
                    "mijn therapeut vermeldde dat", "mijn levenscoach stond erop dat",
                    "mijn dokter zei dat", "mijn intuïtie zegt me dat", "mijn oma waarschuwde me dat",
                    "een droom onthulde dat", "de sterren geven aan dat", "mijn karma suggereert dat"
                ],
                [
                    "heeft me nodig voor emotionele steun.", "waarschuwde me specifiek.",
                    "oordeelt me te hard.", "verwijderde al mijn motivatie.",
                    "zei dat ik moest slapen.", "vereist mijn onmiddellijke aanwezigheid.",
                    "voorspelde een ramp als ik het doe.", "eist dat ik thuis blijf.",
                    "at mijn autosleutels op.", "veroorzaakt tijdelijke allergieën.",
                    "interfereert met mijn aura.", "heeft een kosmisch roosterconflict gecreëerd."
                ]
            ),
            "cs" => (
                [
                    "Nemůžu, protože", "Promiň, ale", "Rád bych, ale",
                    "Bohužel,", "Zkoušel jsem, ale", "Je to nemožné, protože",
                    "Musím odmítnout, protože", "Tentokrát ne, protože", "Chtěl bych, ale"
                ],
                [
                    "moje zlatá rybka", "Merkur je retrográdní a", "můj horoskop řekl, že",
                    "záhadný cizinec", "moje WiFi", "moje rostliny", "sousedova kočka",
                    "můj terapeut zmínil, že", "můj životní kouč trval na tom, že",
                    "můj doktor mi řekl, že", "moje intuice mi říká, že", "babička mě varovala, že",
                    "sen mi odhalil, že", "hvězdy naznačují, že", "moje karma naznačuje, že"
                ],
                [
                    "mě potřebuje pro emocionální podporu.", "mě konkrétně varoval.",
                    "mě příliš soudí.", "smazal veškerou mou motivaci.",
                    "mi řekl, abych si zdříml.", "vyžaduje mou okamžitou přítomnost.",
                    "předpověděl katastrofu, pokud to udělám.", "vyžaduje, abych zůstal doma.",
                    "snědl moje klíče od auta.", "způsobuje dočasné alergie.",
                    "ruší mou auru.", "vytvořil kosmický konflikt v rozvrhu."
                ]
            ),
            "uk" => (
                [
                    "Я не можу, бо", "Вибачте, але", "Я б хотів, але",
                    "На жаль,", "Я спробував, але", "Це неможливо, оскільки",
                    "Я мушу відмовитися, бо", "Не цього разу, бо", "Я б із задоволенням, але"
                ],
                [
                    "моя золота рибка", "Меркурій у ретрограді і", "мій гороскоп сказав, що",
                    "загадковий незнайомець", "мій WiFi", "мої рослини", "кіт сусіда",
                    "мій терапевт згадав, що", "мій лайф-коуч наполягав, що",
                    "мій лікар сказав мені, що", "моя інтуїція підказує, що", "моя бабуся попередила, що",
                    "сон відкрив мені, що", "зірки вказують, що", "моя карма натякає, що",
                    "печиво з передбаченням попередило, що", "моя кришталева куля показала, що"
                ],
                [
                    "потребує мене для емоційної підтримки.", "конкретно застеріг від цього.",
                    "занадто суворо судить мене зараз.", "видалив всю мою мотивацію.",
                    "сказав мені вздрімнути натомість.", "потребує моєї негайної присутності.",
                    "передбачив катастрофу, якщо я це зроблю.", "вимагає, щоб я залишився вдома.",
                    "з'їв мої ключі від машини.", "викликає тимчасову алергію.",
                    "заважає моїй аурі.", "створив космічний конфлікт у розкладі.",
                    "надіслав мені знак уникнути цього.", "блокує мою чакру продуктивності."
                ]
            ),
            _ => ( // English
                [
                    "I can't do that because", "Sorry, but", "I would, but",
                    "Unfortunately,", "I tried, but", "It's impossible since",
                    "I must decline because", "Not this time because", "I'd love to, but"
                ],
                [
                    "my pet goldfish", "Mercury is in retrograde and", "my horoscope said",
                    "a mysterious stranger", "my WiFi", "my plants", "my neighbor's cat",
                    "my therapist mentioned that", "my life coach insisted that",
                    "my doctor told me that", "my gut feeling tells me that", "my grandma warned me that",
                    "a dream revealed that", "the stars indicate that", "my karma suggests that",
                    "a fortune cookie predicted that", "my crystal ball showed that"
                ],
                [
                    "needs me for emotional support.", "specifically warned against it.",
                    "is judging me too hard right now.", "deleted all my motivation.",
                    "told me to take a nap instead.", "requires my immediate presence.",
                    "predicted catastrophe if I do it.", "demands I stay home.",
                    "ate my car keys.", "is causing temporary allergies.",
                    "is interfering with my aura.", "has created a cosmic schedule conflict.",
                    "has sent me a sign to avoid this.", "is blocking my productivity chakra."
                ]
            )
        };
    }
}
