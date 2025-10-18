# Phase 2: Vocabulary System - Complete!

The AdventureEngine now has a full vocabulary system with synonym support, adjective matching, and semantic object resolution!

## What's New in Phase 2

### 1. Database-Driven Vocabulary (130+ words)
All word knowledge is stored in the database, not hardcoded:

**Vocabulary Table:**
- Verbs: take/get/grab/pick (all synonyms)
- Adjectives: golden/gold, rusty/rust, brass, wooden/wood
- Nouns: lamp/lantern/light, sword/blade, book/tome
- Directions: n→north, s→south, etc.

### 2. Item Adjectives
Items now have descriptive adjectives with priorities:

```
Golden Key:
  - "golden" (priority 3 - most distinctive)
  - "ornate" (priority 2)

Brass Lantern:
  - "brass" (priority 2)
  - "old" (priority 1)
```

### 3. Semantic Object Resolution
The `SemanticResolver` intelligently matches player input to game objects:

**Synonym Resolution:**
- "get lamp" → "take lamp" (verb synonym)
- "grab lantern" → "take lamp" (verb + noun synonym)
- "take light" → "take lamp" (noun synonym)

**Adjective Matching:**
- "take golden key" → Matches item with "golden" adjective
- "take brass lantern" → Matches item with "brass" adjective
- "take old lamp" → Matches item with "old" adjective

**Smart Fallback:**
- If exact adjective match fails, fallsback to noun-only match
- "take key" works even if there's a "golden key"

## Examples of What Now Works

### Synonym Commands
```
Instead of:          You can now type:
"take lamp"          "get lamp", "grab lamp", "pick lamp", "acquire lamp"
"examine statue"     "inspect statue", "check statue", "study statue"
"drop sword"         "place sword", "put sword", "leave sword"
```

### Adjective-Based Item Selection
```
Room has: "Lantern" and "Ancient Book"

> take brass lantern
Taken: Lantern

> get ancient book
Taken: Ancient Book

> examine old lamp
An old brass lantern. It still has oil and works perfectly.
```

### Noun Synonyms
```
Item Name: "Lantern"
Vocabulary: lamp, lantern, light (all resolve to same item)

> take lamp
Taken: Lantern

> use lantern
The lantern illuminates the darkness around you.

> drop light
You drop the Lantern.
```

### Combined: Synonyms + Adjectives
```
> get the golden key
Taken: Golden Key

> grab brass lamp
Taken: Lantern

> pick ancient tome
Taken: Ancient Book
```

## Technical Implementation

### Models
- **Vocabulary** - Word definitions with types and canonical forms
- **ItemAdjective** - Links items to their descriptive adjectives

### Services
- **VocabularySeeder** - Seeds 130+ words on first run
- **SemanticResolver** - Resolves player text to game objects
  - `NormalizeVerbAsync()` - Converts verbs to canonical form
  - `NormalizeAdjectiveAsync()` - Converts adjectives to canonical form
  - `NormalizeNounAsync()` - Converts nouns to canonical form
  - `ResolveItemAsync()` - Finds items using smart matching
  - `ResolveExaminableObjectAsync()` - Finds examinable objects

### Updated Commands
All item-based commands now use SemanticResolver:
- **TakeCommand** - "get the golden key"
- **DropCommand** - "drop brass lamp"
- **UseCommand** - "use ornate key on bookshelf"
- **ExamineCommand** - "inspect ancient book"

## Current Vocabulary Coverage

### Verbs (Action Words)
- **Movement**: go, walk, move, travel
- **Acquisition**: take, get, grab, pick, acquire, obtain
- **Placement**: drop, place, put, set, leave
- **Examination**: look, examine, inspect, check, view, observe, study
- **Interaction**: use, activate, apply, employ
- **System**: quit, exit, help, inventory

### Adjectives (Descriptive Words)
- **Colors**: golden, rusty, silver, bronze
- **Materials**: brass, wooden, stone, iron, steel
- **Size**: small, large, big, tiny, huge
- **Age**: old, ancient, new
- **Quality**: ornate, simple, heavy, light, dark, bright

### Nouns (Object Names)
- **Tools**: key
- **Light Sources**: lamp, lantern, light
- **Readable**: book, tome
- **Weapons**: sword, blade
- **Containers**: box, chest
- **Furniture**: bookshelf, shelf
- **Decoration**: statue
- **Barriers**: door

### Directions
- Full: north, south, east, west, up, down
- Short: n→north, s→south, e→east, w→west, u→up, d→down

## Extending the Vocabulary

### Adding New Words
Simply add to VocabularySeeder.cs:

```csharp
vocabularies.Add(new Vocabulary {
    Word = "torch",
    WordType = WordTypes.Noun,
    Category = "light",
    CanonicalForm = "lamp"  // Makes "torch" resolve to "lamp"
});
```

### Adding Item Adjectives
In DatabaseSeeder.cs when creating items:

```csharp
new ItemAdjective {
    ItemId = myItem.Id,
    Adjective = "shiny",
    Priority = 2
}
```

## Performance

- **Vocabulary Lookup**: O(1) database query with index
- **Semantic Resolution**: O(n) where n = items in context (typically <20)
- **Adjective Matching**: O(m) where m = adjectives per item (typically 2-3)

All vocabulary data is cached by EF Core, so subsequent lookups are fast.

## What's Next?

**Phase 3 Options:**
1. **Ambiguity Handling** - "Which key: the golden key or the rusty key?"
2. **Context-Aware Resolution** - Remember what player was talking about
3. **Fuzzy Matching** - Handle typos and partial words
4. **AI-Enhanced Parser** - Use ML for even smarter parsing

## Backward Compatibility

✅ All Phase 1 commands still work
✅ Simple commands like "take lamp" work exactly as before
✅ New features are additive, not breaking

The vocabulary system enhances the engine without changing existing functionality!
