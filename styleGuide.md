Style Guide
За основу Style Guide был взят кодстайл Microsoft

General
Код должен быть простым, ясным и читаемым, в той мере, в которой это возможно для реализации требуемой функциональности.
Код должен быть самодокументируемым. Комментируйте только неочевидные вещи – костыли, воркэраунды, почему именно так сделано.
Нужно стремиться к минимизации статических методов и классов.
Naming
Имена должны быть читабельными и понятными, без сокращений и грамматических ошибок. Исключение – общепринятые сокращения, напр. i, j, k в циклах
// ПЛОХО
var a = new List<Student>();
var stds = new List<Student>();
var list = new List<Student>();

// ХОРОШО
var students = new List<Student>();
Не используйте сокращения
Не используйте отрицание в названиях
bool isRed = color == Colors.Red; // Good
bool isNotBlue = color != Colors.Blue; // Bad

if (!isRed // Ok
&& !isNotBlue) // It is not ok
return ...
Используйте studentsCount и studentsList вместо numberOfStudents, listOfStudents
Придерживайтесь нейминга в соответствии с A/HC/LC Pattern'ом.
Имя состоит из prefix? + action (A) + high context (HC) + low context? (LC).

- Action - это действие, которое выполняется (get, set, remove, etc.)
- HC и LC - это выделение более значимых элементов. Название метода GetUserAssignments подразумевает, что есть основная сущность (User) и ассоциированные зависимые (Assignments).

Prefixes
Методы, предназначенные для доступа к данным, в результате которых может вернуться null, должны иметь префикс Find.
// ХОРОШО
public Student? FindStudent(int id)
{
/_ ищем студента _/

// возвращаем студента вне зависимости от того, null это или нет
return student;
}
Методы, предназначенные для доступа к данным всегда возвращающие ненулевое значение, должны иметь префикс Get (и бросать ошибку, если не могут вернуть не null).
Правильно:

public Person? FindEldestChild()
{
// If \_children in empty - returns null
return \_children
.OrderByDescending(p => p.BirthDate)
.SingleOrDefault();
}
Неправильно:

public Person? FindEldestChild()
{
// If \_children in empty - throws exception
return \_children
.OrderByDescending(p => p.BirthDate)
.Single();
}
Методы, пытающиеся выполнить действие, но не обязательно выполняющие его, должны иметь префикс Try. Мы должны иметь возможность узнать статус выполнения (например bool флаг на выходе из метода)
Правильно:

public bool TryWithdrawMoney(CreditCard creditCard, int password, double moneyToWithdraw)
{
// если пароль не верный, возвращаем false
if (!creditCard.IsPasswordCorrect(password))
return false;

// если пароль верный, то снимаем деньги
creditCard.Withdraw(moneyToWithdraw);

return true;
}
Неправильно:

public void TryWithdrawMoney(CreditCard creditCard, int password, double moneyToWithdraw)
{
// если пароль совпадает, снимаем деньги
if (creditCard.IsPasswordCorrect(password))
creditCard.Withdraw(moneyToWithdraw);
}
Formatting
Добавляйте переносы строк до и после многострочных элементов кода (кроме случаев когда элемент находится в конце блока). Однострочные элементы кода можно группировать без пропуска строк.
Правильно:

var location = "Yerevan";
var age = 69;

var relevantStudents = \_students
.Where(x => x.Location.Equals(location))
.Where(x => x.Age.Equals(age));

foreach (var student in relevantStudents)
{
Console.WriteLine($"Найден дед - {student.Name}");
}
Неправильно:

var location = "Yerevan";
var age = 69;
var relevantStudents = \_students
.Where(x => x.Location.Equals(location))
.Where(x => x.Age.Equals(age));
foreach (var student in relevantStudents)
{
Console.WriteLine($"Найден дед - {student.Name}");
}
Логически группируйте члены типа, отделяя группы переносами строк. (При больших размерах групп, можно выделять подгруппы, по смысловому признаку, отделяя подгруппы пропусками строк)
Правильно:

public class Student
{
public int Id { get; }
public string FirstName { get; }
public string LastName { get; }

public string FullName => $"{FirstName} {LastName}";

public override string ToString()
=> $"[{Id}] {FullName}";
}
Неправильно:

public class Student
{
public int Id { get; }
public string FirstName { get; }
public string LastName { get; }
public string FullName => $"{FirstName} {LastName}";
public override string ToString()
=> $"[{Id}] {FullName}";
}
public class Student
{
public int Id { get; }

public string FirstName { get; }

public string LastName { get; }

public string FullName => $"{FirstName} {LastName}";

public override string ToString()
=> $"[{Id}] {FullName}";
}
Добавляйте перенос строк перед оператором return.
Правильно:

public int Calculate()
{
var x = 420 \* 228;
var y = x / 1337d;

return x + y;
}
Неправильно:

public class Student
public int Calculate()
{
var x = 420 \* 228;
var y = x / 1337d;
return x + y;
}
Variable and operator declaration
Для неочевидных числовых значений необходимо создавать именованные константы. Не используйте в коде магические числа.
Правильно:

public Student AddStudent(string name, string surname)
{
var student = new Student(name, surname);

// сравниваем с читаемой константов
if (\_students.Count >= MaxStudentsAmount)
throw new Exception("Students limit exceeded");

\_students.Add(stident);

return student;
}
Неправильно:

public Student AddStudent(string name, string surname)
{
var student = new Student(name, surname);

// сравниваем с магическим числом
if (\_students.Count >= 20)
throw new Exception("Students limit exceeded");

\_students.Add(stident);

return student;
}
Используйте var, только если тип переменной понятен из контекста.
Правильно:

double percents = bankAccount.CalculatePercents();
decimal percents = bankAccount.CalculatePercents();
Money percents = bankAccount.CalculatePercents();
var studentInfo = new UserInfo(); // тип переменной понятен, тк это инициализация
Неправильно:

var percents = bankAccount.CalculatePercents();
Задавайте default в операторе switch. Если поведение не определено - кидайте исключение.
Правильно:

switch (deposit)
{
case < LowerThreshold:
Console.WriteLine("Your percent is 3");
break;
case < MiddleThreshold:
Console.WriteLine("Your percent is 5");
break;
default:
throw new Exception("Unexpected case");
}
Неправильно:

switch (deposit)
{
case < LowerThreshold:
Console.WriteLine("Your percent is 3");
break;
case < MiddleThreshold:
Console.WriteLine("Your percent is 5");
break;
}
При сравнении переменной с константой сначала указывается переменная, потом константа.
// ПЛОХО
if (MaxStudentsAmount <= \_students.Count)
throw new Exception("Students limit exceeded");

// ХОРОШО
if (\_students.Count >= MaxStudentsAmount)
throw new Exception("Students limit exceeded");
Локальные переменные должны располагаться как можно ближе к месту использования.
// ПЛОХО
public void SomeCalculations(List<int> nubmers)
{
var oddOnly = new List<int>();
var oddOnlyUnique = new List<int>();
var oddOnlyUniqueLimited = new List<int>();
int numbersCount;

oddOnly.AddRange(numbers.Where(...));

// какие-то вычисления

oddOnlyUnique.AddRange(oddOnly.Where(...));

// какие-то вычисления

oddOnlyUniqueLimited.AddRange(oddOnlyUnique.Take(...));
}

// УЖЕ ЛУЧШЕ
public void SomeCalculations(List<int> nubmers)
{
var oddOnly = numbers.Where(...);

// какие-то вычисления

var oddOnlyUnique = oddOnly.Where(...));

// какие-то вычисления

var oddOnlyUniqueLimited = oddOnlyUnique.Take(...);
var numbersCount = oddOnlyUniqueLimited.Count();
}
Минимизируйте уровень вложенности, где это возможно без потери читаемости. Этого можно добиться инвертированием условного оператора if, декомпозицией логики. https://refactoring.com/catalog/replaceNestedConditionalWithGuardClauses.html.
// не очень ХОРОШО
if (\_students.Count >= MaxStudentsAmount)
{
throw new Exception("Students limit exceeded");
}
else
{
\_students.Add(stident);

return student;
}

// ХОРОШО
if (\_students.Count >= MaxStudentsAmount)
throw new Exception("Students limit exceeded");

\_students.Add(stident);

return student;
Не используйте boolean флаги для того, чтобы управлять условиями выхода из цикла.
// ПЛОХО (не нада так пажалуста)
while (true)
{
// что угодно тут будет плохо :)
// особенно, если не будет break;
}

// ЛУЧШЕ
for (int i = 0; i < studentsConut; ++i)
{
// почти всё что угодно тут будет лучше чем предыдущий вариант
// особенно, если тут не будет while (true)
}
Method declaration
Метод, возвращающий коллекцию, в случае отсутствия элементов для возврата, должен возвращать пустую коллекцию, а не null.
// ПЛОХО
public List<Student> FindStudents(int course)
{
// объявляем лист (но не инициализируем)
List<Student> students;

/_ ищем студентов любым возможным методом _/

// если студенты не нашлись, возвращаем null
if (students.Count is null)
return null;

// возвращаем студентов, если хоть кто-то нашёлся
return students;
}

// ХОРОШО
public List<Student> FindStudents(int course)
{
// создаём пустой лист
var students = new List<Student>();

/_ ищем студентов любым возможным методом _/

// возвращаем студентов даже, если это пустой лист (не null)
return students;
}
Метод, который работает с пользовательскими аргументами, должен валидировать их.
// ПЛОХО
public void FindStudentByFullName(string name, string surname)
{
/_ поиск студента без проверки входных данных _/
}

// ХОРОШО
public void FindStudentByFullName(string name, string surname)
{
if (string.IsNullOrWhiteSpace(name))
throw new ArgumentException("Name to find student is empty");

if (string.IsNullOrWhiteSpace(name))
throw new ArgumentException("Surname to find student is empty");

/_ поиск студента после проверки входных данных _/
}
В конструкторе должен соблюдаться порядок инициализации:
Валидация аргументов
Инициализация, которая не зависит от аргументов
Инициализация полей аргументами
Инициализация, которая требует какой-то логики, вызовов методов
// ХОРОШО
public MegaFaculty(string facultyName)
{
// валидация
if (string.IsNullOrWhiteSpace(facultyName))
throw new ArgumentException("Mega faculty name is empty");

// инициализация, не зависящая от аргументов
\_courses = new List<OgnpCourse>();

// инициализация полей аргументами
Name = facultyName;

/_ сложная инициализация с вызовом различных методов _/
NotifyISU(this);
}
Property declaration
При объявлении автосвойств, помещайте аксессоры на одной строке с названием и типом
// ХОРОШО
public int Value { get; set; }

// ПЛОХО
public int Value
{ get; set; }
При объявлении get-only свойств, используйте bodied expressions вместо явного get аксессора
// ХОРОШО
public IReadOnlyCollection<string> Values => \_values;

// ПЛОХО
public IReadOnlyCollection<string> Values
{
get
{
return \_values;
}
}

public IReadOnlyCollection<string> Values
{
get => \_values;
}
Type declaration
Инициализация полей должна происходить в конструкторе, а не при их определении.
Конструкторы должны полностью инициализировать объект. Валидация аргументов должна происходить в конструкторах.
Минимизируйте область доступа к данным. Предпочтительней хранить информацию в приватных полях нежели в публичных свойствах. Методы, которые не нужны внешнему коду, нужно делать приватными.
Не оставляйте мутабельные поля для отложенной инициализации. Инициализируйте поля в конструкторах и делайте иммутабельные поля и свойства, где это уместно.
Не использовать поля для передачи данных внутри метода или между методами класса.
Поддерживайте инвариант типа. Если у типа есть несколько полей, которые между собой связаны, то не должно быть способа изменить одно из полей и нарушить связь между ними.
Члены класса должны располагаться в следующем порядке:
Константы
Поля
Конструкторы и Create-методы
Свойства
Публичные методы
Приватные методы
Нумерация значений енама должна начинаться с 1. 0 должен быть использован для неопределённых значений.
Не используйте приватные свойства.
Не используйте публичные поля.
Не используйте оператор == для сравнения не числовых типов. Не переопределяйте оператор == для не числовых типов.
Не используйте наследование для переиспользования логики. Если объект наследуется, то справедливым должно быть высказывание, что производный объект является базовым (см. LSP).
Exceptions
Все производные от Exception классы должны иметь постфикс "Exception".
Для ошибок бизнес логики стоит бросать кастомный эксепшен. Для стандартных ошибок, например, невалидных аргументов, стоит использовать стандартные типы.
Нужно обрабатывать, где это оправдано, ошибки NRE, OutOfRange etc и вместо них бросать более понятные ошибки, которые описывают проблемную ситуацию.
Если ошибка не может быть обработана, то её необходимо прокидывать дальше, а не игнорировать.
Если возникает ошибка при валидации аргументов, то нужно указывать, какой именно аргумент приводит к ошибке в тексте ошибки.
Common
Методы расширения должны выделяться в специальные классы. Они должны иметь соответствующий постфикс Extensions.
Весь исходный код должен быть написан на английском. Это касается нейминга, комментариев и ошибок. Если есть необходимость использовать другой язык, то нужно применить инструменты локализации.
Для обозначения отсутствия значения стоит использовать null, а не default. Для значимых типов стоит возвращать Nullable.
Избегайте кастов там, где можно их не использовать. Программа должна стремиться к повышению типизации и увеличению количества мест, где происходят проверки во время компиляции.
Минимизируйте количество ап кастов. Старайтесь не использовать более общие типы в сигнатурах, если они не поддерживаются.
При написании цепочки вызовов методов, переносите каждый вызов на отдельную строку.
Для проверки на null использовать конструкции is null и is not null.
Используйте Type.Parse вместо Convert.ToType (например, int.Parse вместо Convert.ToInt32).
Название namespace должно содержать название проекта и все папки через точку, ведущие от корня проекта к текущему файлу. Пример структуры проекта:
Project
Users
Models
Entities
Student.cs
Services
Orders
Models
Entities
Services
При такой структуре проекта название namespace в файле Student.cs будет Project.Users.Entities
Restrict
Не используйте dynamic.
Не используйте goto.
Не пишите касты для своих типов - implicit или explicit.
Не используйте публичные вложенные типы.
Не используйте модификатор доступа internal. Исключение - если нет возможности использовать другой.
Не используйте reflection для доступа к полям.
Не используйте query-like LINQ.
Не используйте Tuple, ValueTuple и KeyValueTuple в сигнатурах своих методов. Вводите специальные типы, которые лучше описывают данную структуру.
Не пишите в лямбдах больше одной операции, выносите сложную логику в методы.
Не используйте char, short для экономии памяти. Большинство интерфейсов работают с int, а значит в коде появится много кастов, которые усложняют код.
Не используйте unsigned для гарантии, что значение будет больше нуля. Большинство интерфейсов работают с int, а значит в коде появится много кастов, которые усложняют код.
Suggestions
При реализации методов создания или поиска, стоит возвращать сущности, а не какие-то их части - названия или идентификаторы. Если написано, что нужно найти магазин, то ожидается, что вернется не строка, а магазин.
Используйте специализированные проверки вида CollectionAssert.Contains(...) вместо Assert.IsTrue(collection.Contains(...)).
Не пишите сложную логику поверх примитивных типов, вводите информативные обёртки, в которых можно инкапсулировать валидацию и логику.
Стоит отделять логику ввода, вывода и бизнес-логику.
Dto
Для простых DTO, где это возможно, стоит использовать record.
Если для DTO используется класс, то сеттеры должны быть приватными.
Если для DTO используется класс, то должен быть публичный пустой конструктор.
Для валидной конвертации из сущности в dto нужно писать мапперы - Expression<Func<TEntity, TDto>>. Они должны напрямую сетить все нужные поля и не использовать конструкторы или статические методы.
