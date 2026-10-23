import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { Note, NoteKind } from '../../core/models/note.model';
import { ShoppingCategory } from '../../core/models/shopping-category.model';
import { NoteService } from '../../core/services/note.service';
import { ShoppingCategoryService } from '../../core/services/shopping-category.service';
import { EmptyState } from '../../shared/empty-state/empty-state';
import { TouchErrorStateMatcher } from '../../shared/touch-error-state-matcher';

interface CategoryGroup {
  id: number | null;
  name: string;
  notes: Note[];
}

@Component({
  selector: 'app-notes',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCheckboxModule,
    MatIconModule,
    MatMenuModule,
    EmptyState,
  ],
  templateUrl: './notes.html',
  styleUrl: './notes.scss',
})
export class NotesPage implements OnInit {
  private readonly noteService = inject(NoteService);
  private readonly categoryService = inject(ShoppingCategoryService);
  private readonly fb = inject(FormBuilder);

  readonly notes = this.noteService.notes;
  readonly categories = this.categoryService.categories;
  readonly kind = signal<NoteKind>('note');
  readonly categoryFilter = signal<number | null>(null);
  readonly creatingCategory = signal(false);
  readonly errorStateMatcher = new TouchErrorStateMatcher();

  readonly title = computed(() => (this.kind() === 'shopping' ? 'Lista de la compra' : 'Notas'));
  readonly placeholder = computed(() =>
    this.kind() === 'shopping' ? 'Añadir a la compra' : 'Escribir una nota',
  );
  readonly emptyMessage = computed(() => {
    if (this.kind() === 'shopping') {
      return this.categoryFilter() === null
        ? 'La lista de la compra está vacía.'
        : 'No hay productos con esta etiqueta.';
    }
    return 'No hay notas todavía.';
  });

  readonly form = this.fb.nonNullable.group({
    content: ['', Validators.required],
  });

  readonly categoryForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
  });

  readonly groupedNotes = computed<CategoryGroup[]>(() => {
    if (this.kind() !== 'shopping') {
      return this.notes().length ? [{ id: null, name: '', notes: this.notes() }] : [];
    }

    const filter = this.categoryFilter();
    const groups = new Map<number | null, Note[]>();

    for (const note of this.notes()) {
      if (filter !== null && note.categoryId !== filter) {
        continue;
      }

      const bucket = groups.get(note.categoryId) ?? [];
      bucket.push(note);
      groups.set(note.categoryId, bucket);
    }

    const result: CategoryGroup[] = [];

    for (const category of this.categories()) {
      const bucket = groups.get(category.id);
      if (bucket?.length) {
        result.push({ id: category.id, name: category.name, notes: bucket });
      }
    }

    const uncategorized = groups.get(null);
    if (uncategorized?.length) {
      result.push({ id: null, name: 'Sin etiqueta', notes: uncategorized });
    }

    return result;
  });

  ngOnInit(): void {
    this.load();
  }

  setKind(event: MatButtonToggleChange): void {
    this.kind.set(event.value as NoteKind);
    this.categoryFilter.set(null);
    this.creatingCategory.set(false);
    this.load();
  }

  selectCategory(id: number | null): void {
    this.categoryFilter.set(id);
  }

  toggleCategoryForm(): void {
    this.creatingCategory.update((value) => !value);
    if (!this.creatingCategory()) {
      this.categoryForm.reset({ name: '' });
    }
  }

  createCategory(): void {
    if (this.categoryForm.invalid) {
      this.categoryForm.markAllAsTouched();
      return;
    }

    const name = this.categoryForm.getRawValue().name.trim();
    if (!name) {
      return;
    }

    this.categoryService.create(name).subscribe(() => {
      this.categoryForm.reset({ name: '' });
      this.creatingCategory.set(false);
      this.reloadCategories();
    });
  }

  deleteCategory(category: ShoppingCategory): void {
    this.categoryService.delete(category.id).subscribe(() => {
      if (this.categoryFilter() === category.id) {
        this.categoryFilter.set(null);
      }
      this.reloadCategories();
      this.load();
    });
  }

  add(): void {
    if (this.form.invalid) {
      return;
    }

    const content = this.form.getRawValue().content.trim();
    if (!content) {
      return;
    }

    this.noteService
      .create({
        content,
        kind: this.kind(),
        roomId: null,
        categoryId: this.kind() === 'shopping' ? this.categoryFilter() : null,
      })
      .subscribe(() => {
        this.form.reset({ content: '' });
        this.load();
      });
  }

  assignCategory(note: Note, categoryId: number | null): void {
    this.noteService
      .update(note.id, {
        content: note.content,
        kind: note.kind,
        roomId: note.roomId,
        categoryId,
      })
      .subscribe(() => this.load());
  }

  categoryName(id: number | null): string {
    if (id === null) {
      return 'Sin etiqueta';
    }
    return this.categories().find((category) => category.id === id)?.name ?? 'Sin etiqueta';
  }

  toggle(id: number): void {
    this.noteService.toggle(id).subscribe(() => this.load());
  }

  remove(id: number): void {
    this.noteService.delete(id).subscribe(() => this.load());
  }

  private load(): void {
    this.noteService.loadAll(this.kind()).subscribe();

    if (this.kind() === 'shopping') {
      this.reloadCategories();
    }
  }

  private reloadCategories(): void {
    this.categoryService.loadAll().subscribe();
  }
}
