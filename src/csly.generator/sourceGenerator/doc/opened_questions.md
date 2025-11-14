
# Opened questions

This page lists all opened questions regarding source generation

## models
We need a minimal set of shared model classes for :
- tokens
- syntax tree
- special EBNF types (Group, ValueOption)

How to include these types ?
- namespace :
    - static (e.g sly.model.generated)
    - ~~by parser (e.g while.sly.model.generated)~~


## Memoization

Memoization will require a dedicated `ParsingContext` as we can not use the legacy one. 
The memoization cache key may be a bit harder to compute , not necessiraly ?
The memoization classes must be generated, this leads to the same question as for the model classes

