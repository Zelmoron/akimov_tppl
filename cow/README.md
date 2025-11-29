

### Команды Make

```bash
# Сборка проекта
make build

# Запуск программы
make run file=<имя_файла.cow>

# Запуск примеров
make run-hello    # Запуск hello.cow
make run-fib      # Запуск fib.cow

# Запуск тестов
make test

# Генерация отчёта о покрытии кода
make coverage

# Очистка
make clean
```

## Примеры программ

### Hello World (hello.cow)

**Код:**
```cow
MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MOO moO MoO moO MoO MoO moO MoO MoO MoO
moO MoO MoO MoO MoO moO MoO MoO MoO MoO MoO moO MoO MoO MoO MoO MoO MoO moO MoO
MoO MoO MoO MoO MoO MoO moO MoO MoO MoO MoO MoO MoO MoO MoO moO MoO MoO MoO MoO
MoO MoO MoO MoO MoO moO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO moO MoO MoO MoO
MoO MoO MoO MoO MoO MoO MoO MoO moO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO
MoO moO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO mOo mOo mOo mOo mOo
mOo mOo mOo mOo mOo mOo mOo mOo MOo moo moO moO moO moO moO moO moO moO MOo MOo
MOo MOo MOo MOo MOo MOo Moo moO moO moO MOo MOo MOo MOo MOo MOo MOo MOo MOo Moo
MoO MoO MoO MoO MoO MoO MoO Moo Moo moO MOo MOo MOo MOo MOo MOo MOo MOo MOo Moo
mOo mOo mOo mOo mOo mOo mOo MOo MOo MOo MOo MOo MOo Moo mOo MOo MOo MOo MOo MOo
MOo MOo MOo Moo moO moO moO moO moO MOo MOo MOo Moo moO moO moO Moo MoO MoO MoO
Moo mOo Moo MOo MOo MOo MOo MOo MOo MOo MOo Moo mOo mOo mOo mOo mOo mOo mOo MoO
Moo
```

**Вывод программы:**
```
Hello, World!
```

### Числа Фибоначчи (fib.cow)

**Описание:** Программа вычисляет первые 11 чисел последовательности Фибоначчи.

**Вывод программы:**
```
1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, ...
```

## Тестирование

Проект включает набор юнит-тестов для проверки корректности работы интерпретатора.

### Результаты тестов

```
Passed!  - Failed: 0, Passed: 16, Skipped: 0, Total: 16
```

**Статус:** ✅ Все 16 тестов пройдены успешно

### Покрытие кода (Code Coverage)

```
+----------------+--------+--------+--------+
| Module         | Line   | Branch | Method |
+----------------+--------+--------+--------+
| CowInterpreter | 80.99% | 77.02% | 75%    |
+----------------+--------+--------+--------+
```

