# Pascal Interpreter

Интерпретатор для упрощённой версии языка Pascal, написанный на C#.

## Описание

Интерпретатор поддерживает:
- Арифметические выражения с операторами `+`, `-`, `*`, `/`
- Унарные операторы `+` и `-`
- Присваивание переменных `:=`
- Вложенные блоки `BEGIN...END`
- Приоритет операций

## Структура проекта

```
.
├── Makefile                                    # Файл для сборки и запуска
├── README.md                                   # Документация
├── PascalInterpreter/                          # Основной проект
│   ├── PascalInterpreter.csproj
│   ├── Program.cs                              # Точка входа с примерами
│   ├── Token.cs                                # Определение токенов
│   ├── Lexer.cs                                # Лексический анализатор
│   ├── AST.cs                                  # Классы узлов AST
│   ├── Parser.cs                               # Синтаксический анализатор
│   └── Interpreter.cs                          # Интерпретатор
└── PascalInterpreter.Tests/                    # Тесты
    ├── PascalInterpreter.Tests.csproj
    └── InterpreterTests.cs                     # Unit-тесты
```

## Требования

- .NET SDK 9.0 или выше
- Make (для использования Makefile)

## Установка .NET SDK (если не установлен)

### macOS
```bash
brew install dotnet
```

### Linux
Следуйте инструкциям на [официальном сайте](https://dotnet.microsoft.com/download)

## Использование

### С помощью Makefile

```bash
# Сборка проекта
make build

# Запуск интерпретатора (выполнит примеры из задания)
make run

# Запуск тестов
make test

# Очистка проекта
make clean

# Справка
make help
```

### Напрямую через dotnet

```bash
# Сборка
dotnet build PascalInterpreter/PascalInterpreter.csproj

# Запуск
dotnet run --project PascalInterpreter/PascalInterpreter.csproj

# Тесты
dotnet test PascalInterpreter.Tests/PascalInterpreter.Tests.csproj
```

## Грамматика

```
program          ::= complex_statement DOT
complex_statement ::= BEGIN statement_list END
statement_list   ::= statement | statement SEMI statement_list
statement        ::= compound_statement | assignment | empty
assignment       ::= variable ASSIGN expr
variable         ::= ID
empty            ::= ''
expr             ::= term (('+' | '-') term)*
term             ::= factor (('*' | '/') factor)*
factor           ::= ('+' | '-') factor | INTEGER | LPAREN expr RPAREN | variable
```

## Примеры программ

### Пример 1: Пустая программа
```pascal
BEGIN
END.
```

### Пример 2: Арифметические выражения
```pascal
BEGIN
    x := 2 + 3 * (2 + 3);
    y := 2 / 2 - 2 + 3 * ((1 + 1) + (1 + 1))
END.
```
Результат:
- x = 17
- y = 11

### Пример 3: Вложенные блоки
```pascal
BEGIN
    y := 2;
    BEGIN
        a := 3;
        a := a;
        b := 10 + a + 10 * y / 4;
        c := a - b
    END;
    x := 11
END.
```
Результат:
- a = 3
- b = 18
- c = -15
- x = 11
- y = 2

## Архитектура

Интерпретатор состоит из следующих компонентов:

1. **Lexer** - преобразует текст программы в последовательность токенов
2. **Parser** - строит абстрактное синтаксическое дерево (AST) на основе токенов
3. **Interpreter** - обходит AST и выполняет программу, вычисляя значения переменных

## Тестирование

Проект содержит 10 unit-тестов, покрывающих:
- Пустую программу
- Арифметические операции
- Вложенные блоки
- Унарные операторы
- Деление (целочисленное и с плавающей точкой)
- Переопределение переменных
- Множественные переменные
- Сложные выражения
- Пустые операторы
- Обработку ошибок (неопределённые переменные)

## Автор

Проект создан как учебное задание для изучения построения интерпретаторов.
