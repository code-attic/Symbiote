using System;
using Lucene.Net.Documents;
using Lucene.Net.Index;

namespace Symbiote.Lucene.Impl
{
    public class LuceneIndexingObserver
        : BaseIndexingObserver
    {
        public override void OnNext(Tuple<string, string> value)
        {
            var fieldType = FieldType.FindNumericFieldType(value.Item2);
            if(fieldType != null)
            {
                document.Add(fieldType.ToField(value.Item1, value.Item2, Field.Store.YES));
                document.Add(new Field(value.Item1, value.Item2, Field.Store.YES, Field.Index.NOT_ANALYZED));
            }
            else
            {
                document.Add(new Field(value.Item1, value.Item2, Field.Store.YES, Field.Index.ANALYZED));
            }
        }

        public override void OnError(Exception error)
        {

        }

        public override void OnCompleted()
        {
            indexWriter.AddDocument(document);
            if(indexWriter.NumRamDocs() > 0)
            {
                indexWriter.Optimize();
                indexWriter.Commit();
            }
        }

        public LuceneIndexingObserver(IndexWriter indexWriter)
            : base(indexWriter)
        {
            this.indexWriter = indexWriter;
            document = new Document();
        }
    }
}