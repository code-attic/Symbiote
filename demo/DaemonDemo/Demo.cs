using System;
using System.Diagnostics;
using Symbiote.Core.Extensions;
using Symbiote.Core.Log;
using Symbiote.Daemon;
using Symbiote.Eidetic;
using Symbiote.Eidetic.Extensions;
using Symbiote.Jackalope;
using Symbiote.Relax;

namespace DaemonDemo
{
    public class Demo : IDaemon
    {
        private ILogger<Demo> _logger;
        private IBus _bus;
        private IDocumentRepository _documents;
        private bool _configured;
        private IRemember rememory;
        private string _message =
@"
Lorem ipsum dolor sit amet, consectetur adipiscing elit. Fusce pharetra, diam in tempus sollicitudin, odio arcu suscipit orci, lobortis tincidunt neque magna eget elit. Nunc elit diam, vehicula ut volutpat id, bibendum id odio. Proin lobortis orci sit amet neque scelerisque a varius ligula pharetra. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Nulla lacinia purus id mauris consequat sit amet aliquam odio iaculis. Phasellus ut vestibulum quam. Vestibulum rhoncus, libero sed faucibus bibendum, lorem massa feugiat libero, id imperdiet sem risus non massa. Aliquam iaculis pharetra auctor. Donec bibendum dictum est sodales blandit. Duis tempus luctus dolor, ut mattis turpis semper et.

Donec adipiscing semper dolor eget sollicitudin. Integer pharetra erat at lorem mollis porttitor vulputate est imperdiet. Maecenas semper lobortis massa, vel facilisis ipsum cursus nec. Fusce tellus nulla, tempus sed porta tempor, lobortis ut justo. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Cras felis arcu, venenatis at iaculis ut, tincidunt nec dui. Nullam id tellus tellus, commodo convallis odio. Cras malesuada lacus tortor, id aliquam urna. Morbi eget tellus dui, a dignissim leo. Curabitur ultricies laoreet neque sit amet lobortis. Mauris sed enim est. Maecenas vitae ipsum sem. Nam pellentesque, mi faucibus faucibus feugiat, nunc orci lacinia est, ac molestie massa leo sit amet lacus.

Mauris venenatis aliquet nunc non varius. Cras tristique leo in velit porttitor sagittis. Vestibulum pulvinar libero sed mauris mattis mattis. Nunc risus nunc, tempus a euismod ac, commodo et erat. Duis mattis augue non nulla commodo sed rutrum libero iaculis. Etiam sit amet erat justo, vitae commodo felis. Morbi in nulla et metus placerat interdum tristique ac odio. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Ut lobortis sollicitudin ipsum, sit amet lacinia augue consectetur in. Aenean iaculis, ligula vel facilisis feugiat, nibh augue posuere tellus, non rutrum elit mi et elit. Nulla fringilla facilisis dolor, ac vulputate quam semper in. Nunc quis nulla felis, et tempus augue. Pellentesque metus tortor, tempus eget tincidunt eget, mattis sit amet lectus. Nunc faucibus metus ut nisl eleifend nec sollicitudin nunc sodales. Aenean at dui id eros posuere elementum. Quisque eros sapien, facilisis et consectetur id, sagittis sed magna. Cras at sem id nulla malesuada laoreet eget ut dolor. Etiam placerat nisi id neque rhoncus molestie. Vestibulum volutpat euismod risus sit amet adipiscing.

Suspendisse sed est quis tortor fermentum viverra. Nunc adipiscing gravida purus, nec tristique libero consectetur eu. Aenean posuere sodales mauris eu pellentesque. Pellentesque varius libero et metus vestibulum tincidunt. Duis facilisis neque et lorem tempor interdum. Nullam dolor velit, tincidunt at adipiscing in, rhoncus iaculis quam. Suspendisse fringilla sem vel est laoreet facilisis. Maecenas sit amet tempus justo. Proin at urna et justo aliquet volutpat quis quis nisi. Nunc rhoncus aliquam est eget semper. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Sed nunc arcu, blandit a cursus sed, sollicitudin ut augue. Fusce hendrerit sagittis ornare.

Pellentesque iaculis urna quis dui malesuada auctor. Maecenas iaculis scelerisque dolor, venenatis rutrum lectus tristique eget. Donec vitae tortor a nisl mollis pretium. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Vestibulum accumsan sodales ligula, at aliquam dolor elementum a. Nunc pharetra commodo mauris, non congue sem tristique in. Nullam faucibus nulla non justo faucibus ut vestibulum neque egestas. Morbi vitae nibh in massa hendrerit ornare. Mauris placerat odio ac risus tincidunt porta. Morbi non consectetur orci.

Pellentesque commodo dolor vitae turpis condimentum facilisis. Quisque mauris nisl, adipiscing in pellentesque non, sollicitudin sed turpis. In hac habitasse platea dictumst. Integer dignissim condimentum turpis quis rhoncus. Sed ligula massa, sagittis sit amet suscipit consequat, cursus tincidunt sem. Vestibulum lacinia enim eget eros rhoncus vel vulputate mauris consequat. Nullam rutrum gravida nunc quis pellentesque. In hac habitasse platea dictumst. Donec enim justo, consequat quis cursus ut, gravida nec lorem. Morbi nisi nisl, molestie eget porta eu, iaculis dignissim sem. Pellentesque sem ante, rutrum sed pellentesque at, eleifend in elit. Integer vestibulum, justo quis aliquam iaculis, tortor sem vulputate elit, nec tempus arcu urna sit amet libero.

Vestibulum convallis vestibulum leo eu eleifend. Nullam tincidunt molestie gravida. Vestibulum ac nisi ut mi sollicitudin feugiat non at nunc. Nullam auctor, nunc id faucibus vulputate, dui nisl lacinia libero, non ultrices nisi ipsum eu magna. Donec dolor tellus, varius blandit viverra vel, pulvinar a dolor. Etiam ante nisi, porttitor vulputate suscipit sit amet, adipiscing sit amet eros. Cras sit amet purus eu enim ullamcorper malesuada a eu odio. Duis semper, eros in facilisis scelerisque, sapien tellus blandit magna, feugiat adipiscing ante tellus in neque. Donec diam diam, convallis id porttitor vel, iaculis id justo. Curabitur ac erat at risus placerat facilisis eu vel sem. Praesent porttitor est ac nulla blandit non bibendum massa pharetra. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Pellentesque at ornare tellus. Vivamus tempus diam vel libero rutrum a lacinia ligula ultricies. Mauris ornare gravida fringilla. Vivamus auctor tincidunt ornare. Nullam quam mauris, rhoncus id sollicitudin quis, vestibulum sed ligula.

Etiam sodales turpis bibendum libero scelerisque malesuada. In feugiat mi lacus. Vivamus sit amet magna vitae nisl imperdiet malesuada. Nullam ac mattis velit. Phasellus quis eros mi, et lobortis ipsum. Praesent vel erat risus, vestibulum viverra ligula. Donec laoreet, purus at consectetur hendrerit, ipsum nunc tristique nisi, at eleifend ipsum magna sed lectus. Sed venenatis, justo sed ullamcorper rhoncus, tellus lorem ultrices eros, ac lobortis metus quam pharetra velit. Aenean iaculis pretium leo suscipit dignissim. Ut eu augue nisi, sit amet euismod justo. Phasellus eros lorem, pharetra faucibus pretium quis, adipiscing placerat ipsum. Phasellus venenatis tristique enim, non ultricies sapien congue vel. Maecenas tellus lectus, fringilla eu vulputate vitae, vestibulum at nulla. Maecenas sapien augue, cursus at accumsan eu, lobortis sit amet diam. Nunc sagittis, felis quis molestie scelerisque, neque enim vehicula erat, quis vulputate ligula nisl ac urna. Sed dapibus lobortis sapien, quis malesuada risus auctor tincidunt.

Suspendisse viverra purus adipiscing ipsum pellentesque et faucibus libero adipiscing. Ut at erat at lectus feugiat vulputate. Quisque non sodales massa. Aenean vehicula varius luctus. Curabitur sit amet arcu arcu, at molestie quam. Cras sit amet augue eget nisl sagittis vehicula. Sed auctor malesuada arcu ac tempor. Nulla massa quam, condimentum quis pharetra vel, dapibus ut leo. Proin a dictum diam. Morbi sed odio at est varius vehicula. Integer turpis libero, cursus at pellentesque at, dictum a mauris. Suspendisse commodo magna ac diam pretium ornare.

Fusce eu pulvinar justo. Mauris at dui purus, et tempus lectus. Etiam pharetra fringilla erat at gravida. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus massa dui, facilisis eu sodales aliquet, mattis ac urna. Vestibulum vitae nunc massa, id bibendum ligula. Sed pharetra nisl enim, vel luctus nunc. Suspendisse potenti. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer id sem in nisi pretium sodales. Ut pharetra pretium nisi, eget tristique lorem bibendum eget. Etiam velit mi, aliquet a venenatis a, tempus a nisl. Aenean gravida, justo et luctus porttitor, diam nisi blandit sapien, at tincidunt risus metus ut purus. Donec hendrerit posuere ultricies. Sed felis quam, suscipit suscipit pharetra sed, fermentum sit amet ligula.

Proin eu sodales purus. Morbi molestie porta molestie. Etiam sit amet leo a lectus iaculis euismod. Nulla ac felis eget tellus interdum blandit. Fusce sollicitudin nulla id massa tincidunt malesuada. Praesent at quam sit amet enim sollicitudin interdum nec ut tellus. Aliquam diam augue, feugiat id laoreet vitae, dapibus non mi. Nunc porttitor, metus in elementum semper, ante sem venenatis lorem, nec commodo nunc libero quis lorem. Duis hendrerit, mi ut faucibus iaculis, dolor nunc accumsan lorem, ut interdum nisl dolor et justo. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos.

Donec id orci in neque ultrices laoreet vitae eget nisi. Proin vitae tortor dolor, sit amet pharetra turpis. Curabitur tempor placerat dolor, in congue arcu tincidunt vel. Vivamus consequat urna vitae lectus aliquet ornare. Aliquam erat volutpat. Nunc aliquam leo pellentesque sem hendrerit id rutrum risus viverra. Praesent mattis metus non elit ornare ut rhoncus turpis ornare. Etiam sollicitudin urna quis dolor tincidunt eget dapibus magna dignissim. Quisque ultrices diam nisi, sed fringilla dui. Sed facilisis rhoncus dignissim. In rhoncus velit non augue porta a auctor sapien sodales. Nulla sem est, posuere eget rhoncus viverra, laoreet eu diam. Aenean vel nisi quis nisl consequat commodo sed vitae metus. Morbi ut augue non lacus molestie scelerisque a eget lacus. Donec eleifend molestie velit. Praesent porta rutrum aliquam. Pellentesque suscipit nunc et diam cursus non ultricies nisi vehicula. Sed sed quam arcu. Praesent pulvinar sollicitudin vestibulum.

Cras nunc nisl, vestibulum in dictum nec, posuere sed est. Sed vel felis nec tellus blandit suscipit. Etiam iaculis sodales nulla, eget tristique tortor auctor non. In eget velit erat. Vivamus ultrices nibh fringilla massa luctus ullamcorper. Donec quis massa nunc. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nulla id dui diam. Mauris faucibus vehicula mi, at tristique lorem congue dictum. Etiam euismod, lectus sit amet dictum ullamcorper, enim nulla ultricies sapien sed.
";

        public void Start()
        {
            _logger.Log(LogLevel.Info, "hello");

            var list = _documents.GetAll<ExchangeRecord>();

            Greet();
        }

        private void Talk()
        {
            "Hi, {0}, nice to meet you."
                .ToInfo<Demo>("name".Remember<string>());

            "{0}, would you like to see some gratuitous messaging?"
                .ToInfo<Demo>("name".Remember<string>());

            var response = Console.ReadLine();
            var seconds = TimeSpan.FromSeconds(3000);
            "message".Remember(r => r.Is(response).For(seconds));
            TestBus();

            var id = "recordId".Remember<Guid>();
            var rememberedDocument = id.ToString().Remember<JsonDocument>();
            
            if(rememberedDocument != null)
            {
                "I remember that {0} is the document id for '{1}'"
                    .ToInfo<Demo>(
                        id,
                        rememberedDocument.Body
                    );    
            }
            else
            {
                "I forgot what the document was... good thing I stored it. Here: '{0}'"
                    .ToInfo<Demo>
                    (
                        _documents.Get<JsonDocument>(id).Body
                    );   
            }
        }

        public void Configure()
        {
            if(!_configured)
            {
                "I was asked to go 'configure' myself.... not sure how to feel about that, y'know?"
                    .ToInfo<Demo>();
                _configured = true;
            }
        }

        private void TestBus()
        {
            int x = 0;
            //while("message".Remember<string>() != null)
            var watch = new Stopwatch();
            watch.Start();
            var records = 1000;
            while(x < records)
            {
                _bus.Send("daemondemo", MessageOne.Create("MessageOne {0}".AsFormat(++x)));
            }
            while(Handler.total < records)
            {
                
            }
            watch.Stop();
            var record = new JsonDocument()
                         {
                             Body = "I just sent and received like ... {0} documents in {1} seconds!".AsFormat(x, watch.Elapsed.TotalSeconds)
                         };
            "recordId".Remember(r => r.Is(record.Id));
            _documents.Save(record);

            var message = MessageTwo.Create("This is message type two");
            _bus.Send("daemondemo", message);
        }

        private void Greet()
        {
            _logger.Log(LogLevel.Info, "Hi, what's your name?");
            "name".Remember(r => r.Is(Console.ReadLine()));
            Talk();
        }

        public void Stop()
        {
            "good-bye"
                .ToInfo<Demo>();
            _bus.DestroyQueue("daemondemo");
        }

        public Demo(ILogger<Demo> logger, IBus bus, IDocumentRepository documents)
        {
            _logger = logger;
            _bus = bus;
            _documents = documents;

            _bus.AddEndPoint(x => x.QueueName("daemondemo").Exchange("daemondemo", ExchangeType.direct));
            _bus.Subscribe("daemondemo", null);
        }
    }

    public class ExchangeRecord : DefaultCouchDocument
    {
        public string ExchangeName { get; set; }
    }
}