# Repository Instructions

## Comment and Documentation Language

- Write all code comments in English.
- Write all XML documentation, Markdown documentation, README content, and other developer-facing documentation in English.
- Do not use Russian or mixed-language comments or documentation.
- Keep terminology consistent across code, comments, and documentation.
- Use clear English names for types, members, variables, files, modules, and public APIs.

## Comment Quality

- Prefer self-explanatory code over excessive comments.
- Add comments only when they explain intent, constraints, assumptions, tradeoffs, or non-obvious behavior.
- Do not add comments that only restate what the code already makes obvious.
- Keep comments concise, accurate, and aligned with the current implementation.
- Update or remove comments when the code changes so documentation never becomes misleading.

## Architecture and Design

- Keep the architecture clean, modular, and easy to maintain.
- Follow SOLID principles in design and implementation.
- Give each class, service, and module a single, well-defined responsibility.
- Prefer composition over inheritance unless inheritance is clearly justified.
- Minimize coupling and keep related behavior cohesive.
- Separate domain logic from UI, infrastructure, persistence, and framework-specific concerns.
- Depend on abstractions at system boundaries when this improves testability, extensibility, or clarity.
- Keep public APIs explicit, stable, and easy to understand.
- Eliminate duplicated logic through extraction or refactoring instead of copying behavior.
- Avoid god objects, hidden side effects, and unclear ownership of responsibilities.

## Coding Workflow

- State assumptions explicitly before implementing when requirements are unclear.
- Ask for clarification when multiple interpretations are plausible and the choice is risky.
- Prefer the simplest implementation that solves the requested problem.
- Do not add speculative features, abstractions, configurability, or error handling.
- Make surgical changes: touch only what is needed for the request.
- Match the existing style, even when a different style would also work.
- Do not refactor or reformat adjacent code unless the task requires it.
- Remove only imports, variables, functions, and files made unused by your own changes.
- Mention unrelated dead code or issues instead of changing them.
- Define verifiable success criteria for non-trivial work.
- Verify changes with focused tests or builds whenever practical.
