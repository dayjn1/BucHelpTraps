﻿namespace BucHelp.DatabaseServices
{
    /// <summary>
    /// Main entrypoint into the database API.
    /// </summary>
    public class Drivers
    {
        private static readonly IDatabaseDriver csvd = new CSVDriver("./../CSVs");

        /// <summary>
        /// Request the default database driver.
        /// </summary>
        /// <returns>
        ///     an instance of the default database driver
        /// </returns>
        public static IDatabaseDriver GetDefaultDriver()
        {
            return csvd;
        }

        public static void APIUseTest()
        {
            IDatabaseDriver driver = Drivers.GetDefaultDriver();
            Console.WriteLine("Creating table");
            driver.CreateTable("duck",new RowHeader(
                new Column("colt",Column.Type.Text),
                new Column("coln",Column.Type.Numeric),
                new Column("coli", Column.Type.Integer),
                new Column("colr", Column.Type.Real)
            ));
            ITable duck = driver.GetTableForName("duck");

            Console.WriteLine("Row 1");
            Row row = new Row(duck.Header);
            row.SetAsString("colt", "foo, bar, \nbaz");
            row.SetAsInt("coln", 1);
            row.SetAsDouble("coln", 3.14);
            row.SetAsFloat("colr", 42.0f);
            row.SetAsLong("coli", long.MaxValue);
            duck.Insert(row);
            Console.WriteLine("Mod row 2");
            row.SetAsString("colt", "second row");
            duck.Insert(row);

            Console.WriteLine("Select where coln == 3");
            foreach (Row r in duck.Select(qr => qr.GetAsInt("coln") == 3))
            {
                Console.WriteLine(r.GetAsString("colt"));
                Console.WriteLine(r.GetAsLong("coln"));
                Console.WriteLine(r.GetAsLong("coli"));
                Console.WriteLine(r.GetAsDouble("colr"));
            }

            Console.WriteLine("Select all");
            foreach (Row r in duck.SelectAll())
            {
                Console.WriteLine(r.GetAsString("colt"));
                Console.WriteLine(r.GetAsLong("coln"));
                Console.WriteLine(r.GetAsLong("coli"));
                Console.WriteLine(r.GetAsDouble("colr"));
            }

            Console.WriteLine("Update");
            duck.Update(qr => qr.GetAsString("colt").Equals("second row"),"colt","updated");

            // note: update can cause duplicate rows - will backlog as needed

            Console.WriteLine("Select all");
            foreach (Row r in duck.SelectAll())
            {
                Console.WriteLine(r.GetAsString("colt"));
                Console.WriteLine(r.GetAsLong("coln"));
                Console.WriteLine(r.GetAsLong("coli"));
                Console.WriteLine(r.GetAsDouble("colr"));
            }

            Console.WriteLine("Commit");
            driver.Commit();
            driver.Dispose();
        }
    }
}
