
using Newtonsoft.Json;
using System.Xml.Serialization;

[Serializable]
public class Figure 
{
    [XmlElement("Name")]
    public string Name { get; set; }
    [XmlElement("Width")]
    public int Width { get; set; }
    [XmlElement("Height")]
    public int Height { get; set; }

    public Figure()
    {
    }

    public Figure(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
    

    public override string ToString()
    {
        Console.WriteLine("Нажмите клавишу Escape для выхода");
        return $"Name: {Name}, Width: {Width}, Heigth: {Height}";
    }
}


class TextEditor
{
    private List<Figure> figures = new List<Figure>();
    private int selectedFigureIndex = 0;

    public void AddFigure(Figure figure)
    {
        figures.Add(figure);
    }

    public void DisplayFigures()
    {
        Console.Clear();
        for (int i = 0; i < figures.Count; i++)
        {
            if (i == selectedFigureIndex)
                Console.WriteLine($"[{i}] {figures[i]}");
            else
                Console.WriteLine($" {i}  {figures[i]}");
        }
    }

    public void EditFigure()
    {
        while (true)
        {
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (selectedFigureIndex > 0)
                        selectedFigureIndex--;
                    break;
                case ConsoleKey.DownArrow:
                    if (selectedFigureIndex < figures.Count - 1)
                        selectedFigureIndex++;
                    break;
                case ConsoleKey.Enter:
                    
                    Console.Write("Name: ");
                    string newName = Console.ReadLine();
                    Console.Write("Width: ");
                    int newWidth = int.Parse(Console.ReadLine());
                    Console.Write("Heigth: ");
                    int newHeight = int.Parse(Console.ReadLine());
                    figures[selectedFigureIndex] = new Figure(newName, newWidth, newHeight);
                    break;
                case ConsoleKey.Escape:
                    return;
            }
            DisplayFigures();
        }
    }

    public void SaveFigures(string filename, string format)
    {
        string fullPath = Path.Combine(Environment.CurrentDirectory, filename);

        if (format == "txt")
        {
            using (StreamWriter writer = new StreamWriter(fullPath))
            {
                foreach (var figure in figures)
                {
                    writer.WriteLine(figure.ToString());
                }
            }
        }
        else if (format == "json")
        {
            string json = JsonConvert.SerializeObject(figures, Formatting.Indented);
            File.WriteAllText(fullPath, json);
        }
        else if (format == "xml")
        {
            
            XmlSerializer serializer = new XmlSerializer(typeof(List<Figure>));
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                serializer.Serialize(fs, figures);
            }
            
            
            
        }
    }


    public void LoadFigures(string filename, string format)
    {
        using (StreamReader reader = new StreamReader(filename))
        {
            if (format == "txt")
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    string name = parts[0].Split(':')[1].Trim();
                    int width = int.Parse(parts[1].Split(':')[1].Trim());
                    int height = int.Parse(parts[2].Split(':')[1].Trim());
                    figures.Add(new Figure(name, width, height));
                }
            }
            else if (format == "json")
            {
                string json = reader.ReadToEnd();
                figures = JsonConvert.DeserializeObject<List<Figure>>(json);
            }
            else if (format == "xml")
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<Figure>));
                using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    figures = (List<Figure>)serializer.Deserialize(fs);
                }

            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        TextEditor editor = new TextEditor();

        Console.Write("Введите путь файла, который нужно открыть: ");
        string filePath = Console.ReadLine();
        Console.Write("Введите пожалуйста формат этого файла (txt, json, xml): ");
        string fileFormat = Console.ReadLine();

        editor.LoadFigures(filePath, fileFormat);

        editor.DisplayFigures();
        editor.EditFigure();

        editor.DisplayFigures();

        Console.Write("Введите путь файла, который нужно сохранить: ");
        string savePath = Console.ReadLine();
        Console.Write("Введите пожалуйста формат этого файла(txt, json, xml): ");
        string saveFormat = Console.ReadLine();

        editor.SaveFigures(savePath, saveFormat);
    }
}

