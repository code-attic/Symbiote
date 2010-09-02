using System;
using System.Collections.Generic;
using System.Threading;
using Symbiote.Core.Extensions;
using Symbiote.Jackalope;
using System.Linq;

namespace PublishDemo
{
    public class Publisher
    {
        private IBus _bus;
        private Action<string, Message> send;

        protected readonly string messageBody = 
            @"
            Lorem ipsum ad pri offendit volutpat, mel aperiri evertitur et. Quo in brute blandit. Convenire hendrerit ne mei, mea ut omnesque eloquentiam. Dictas abhorreant has ea, eum ne saepe facete.

 Eu per fugit nostro mediocritatem, omnis dolor his et. In eam postea hendrerit, pro ut hinc corpora. Et per doctus invidunt takimata, mazim inermis adolescens qui eu, solum voluptua consectetuer mea cu. Sed minimum ancillae id, ei pri vocent torquatos. Adhuc tation nonumy mei ne. Detraxit tractatos qualisque et eam, id vim ridens moderatius. Mei brute delectus volutpat at.

 Mea appareat abhorreant ut, aliquip philosophia no eos. Postea blandit menandri ad quo, pro in dico propriae. Liberavisse definitionem ad qui, partem incorrupte constituam pro in, tibique intellegam signiferumque sit ad. Cum eu nibh praesent, ut movet instructior mel. Est adipisci vulputate ex, ut veritus comprehensam mei.

 Salutatus dissentias sit ad. Ne per novum laboramus. No sed brute oblique oporteat, augue sensibus antiopam mel et, dicta minimum et duo. Sed et dicit prodesset maiestatis, no velit sonet mucius qui. Nec at nihil dolorem forensibus.

 Ius augue ubique signiferumque ne, dicit deserunt tractatos et has, id sea libris feugiat accommodare. Ea brute doming ocurreret has, putant everti utroque vim te. Nam accusamus dissentias in, ea pro denique convenire delicatissimi. Nam ex ceteros pericula, admodum consequat his ei, vis at mollis moderatius.

 Kasd movet an sea, ipsum democritum disputando nec ex. Nibh kasd mei at, quidam debitis abhorreant id pro. Te mea simul aliquam. Saperet temporibus instructior est eu, legere dolores sit ea, fabulas aliquando cum eu. Ne quas consequuntur mea, augue choro quaestio ad vix.

 Mei in elit alterum. Te veri option honestatis vim. Agam vidit mei et. Expetenda pertinacia interpretaris ad eum. Id vim novum electram. Indoctum deterruisset nam ex, id has apeirian quaestio deseruisse, ne sed mundi mediocrem.

 Ea meliore phaedrum eam, mei nemore consequat eu. An inani ornatus partiendo vix. In rationibus incorrupte vel. Quo illud homero voluptua an, modus verear platonem qui eu.

 Dolore prompta in eum, ei oblique euismod vel. Melius aliquando mel ei, soluta mandamus mei te, vocibus indoctum dissentiet vis ad. Te omnis persecuti pertinacia mel. Eu minim vocibus his, eum habeo choro latine ne, illum tantas ad est. Cu legere accommodare vel, doming assueverit ea mea.

 Cum movet expetendis ei, sea error elitr te. Ei duis graecis ponderum sea, sea minim percipit suavitate in. Assum regione pri in. Cu ponderum argumentum pro, usu doming eligendi ea. No stet mutat nostro pro, suas cibo periculis per at. Mutat tamquam menandri ei eum, ut causae aliquip utroque sea, omnes altera doming ad mea. Platonem rationibus ne vis, per id simul minimum commune.

 His ex civibus eligendi, eu qui amet ferri delenit, erant persecuti at eum. Cum eu populo laoreet, ad mei possim molestie atomorum, vis ut inermis aliquyam. Sea cu nibh timeam cotidieque, pro te dictas molestie adipiscing. Denique consequat sed an. Congue tritani admodum eum ne.

 Iusto epicuri voluptatibus eos in. Ea ius facer nusquam mediocrem. Graeco malorum no eum. Eu eos delicatissimi concludaturque, ferri prompta ut nec. Quem animal has in. No sed cibo quando perpetua. Duo facer affert ei.

 Nam viris tritani in, ut nam omnes forensibus posidonium, nec at brute recteque. Ad feugait similique sit. Ea sed possim suscipiantur, ex vim eirmod lobortis argumentum, et duo hinc delectus quaestio. Ex dicta denique per, scaevola percipitur usu cu.

 At his luptatum salutatus, vix dolorem rationibus ne. Oratio omnium legendos vim in, an tollit nusquam indoctum mei, consulatu torquatos consetetur ea vix. Ex vel aperiri admodum invenire. Dolorem sapientem eum ei. Eam nonummy ocurreret in.

 Nam ad cibo impedit voluptatibus, vis no oporteat persecuti. Pri id labitur inermis suscipiantur. Quo vide legere splendide ne, et scripta graecis mei. Eum cu erant affert percipitur, assum principes vel ea, tota senserit ius ex. At vidit atqui populo sit, duo et lucilius constituam. Ut vel zzril lucilius, ad quo vidit corpora.

 Cum in autem appetere recusabo, nam in elit mundi dicam. Sea propriae intellegat dissentiet et, sumo solum concludaturque eum te. Ad nec quem vivendum, maluisset dissentias signiferumque eum ea. Meliore dissentiunt no eos. Pro eu prima debitis incorrupte. Viris impetus aperiam et has, eam at nominati concludaturque conclusionemque. Congue similique per ex, an audiam efficiantur vis.

 Id nihil eligendi assueverit mei. Cum modus omnes feugait ea. Nam cu dolorum necessitatibus. Nonumy mentitum cu sea.

 Et sit cibo eius virtute, munere tempor consequat ad mel, mel cu lorem quando fuisset. Rebum modus eu vix. Te sit utroque accusam expetenda. Eos ei regione fabulas efficiantur. Duo nullam nemore prompta id, nominati sensibus reprehendunt no vix.

 Forensibus dissentiet sea in, laudem reprimique sea te, cu gubergren instructior per. Sed quod fastidii at, errem quidam adipiscing vim in, est semper iisque menandri ne. Sonet numquam concludaturque eos ei, ipsum ullum appetere vim te. No modo harum convenire has, et eos vide numquam impedit, id ius sumo atomorum laboramus. Utroque constituto mea eu, an sit eius accumsan.

 Cu nullam alterum ius, nam vivendum abhorreant deseruisse in, forensibus contentiones sit te. Dicunt alienum ad nec, vix ea suas gubergren. Quo ex dicta utamur, vim ad insolens adipiscing, cu graeci impetus eam. Qui ex velit utinam dolorum, ad sonet alterum periculis sit. Qui graece quidam alterum ex, falli erant euismod ei nam. Viris senserit maluisset his ex, sed id commune noluisse mandamus.

 Nec an etiam iusto numquam, id quot facilisi mei. His ad cetero lobortis. Eam ad solet iudico persequeris, mea cu wisi quando complectitur. Eum wisi accumsan antiopam eu, eam ad debet essent. Id nihil philosophia per, ne rebum perpetua has, ad puto legere duo.

 Sea aliquip conclusionemque cu. Ei errem vidisse prompta mei. Dolorum legimus ut usu, ex mea nonumy postulant erroribus. Illum fabulas mea ea, has puto facilis et. Modus alienum volutpat cum at, ei sit luptatum perfecto. Nec eros aperiam ut, nam quidam deseruisse eu.

 Qui te omnes alienum, eu vero deleniti consulatu sit. Has ne solum probatus, pro gloriatur deterruisset ei. His enim atqui dissentiunt an, fabulas suscipiantur eu sit. Tation incorrupte eu ius.

 Oportere intellegam est an.
        ";

        public void Start()
        {
            //"Publisher is starting.".ToInfo<Publisher>();
            //long i = 0;
            //var message = new Message("Message");
            //while(true)
            //{
            //    send.BeginInvoke("publisher", message, null, null);
            //}
        }

        public Publisher(IBus bus)
        {
            _bus = bus;
            _bus.AddEndPoint(x => x.Exchange("publisher", ExchangeType.fanout));
            _bus.AddEndPoint(x => x.Exchange("secondary", ExchangeType.fanout));
            _bus.DefineRouteFor<Message>(x => x.SendTo("publisher"));
            var observable = Observable.Generate(0, x => x < 500000, x => new Message(messageBody), x => x + 1);
            observable.ToEnumerable().AsParallel().ForAll(_bus.Send);
            //_bus.AutoRouteFromSource(observable);
            send = bus.Send;
        }
    }
}