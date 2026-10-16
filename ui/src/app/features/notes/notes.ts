import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleChange, MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { NoteKind } from '../../core/models/note.model';
import { NoteService } from '../../core/services/note.service';
import { EmptyState } from '../../shared/empty-state/empty-state';

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
    EmptyState,
  ],
  templateUrl: './notes.html',
  styleUrl: './notes.scss',
})
export class NotesPage implements OnInit {
  private readonly noteService = inject(NoteService);
  private readonly fb = inject(FormBuilder);

  readonly notes = this.noteService.notes;
  readonly kind = signal<NoteKind>('note');

  readonly title = computed(() => (this.kind() === 'shopping' ? 'Lista de la compra' : 'Notas'));
  readonly placeholder = computed(() =>
    this.kind() === 'shopping' ? 'Añadir a la compra' : 'Escribir una nota',
  );
  readonly emptyMessage = computed(() =>
    this.kind() === 'shopping' ? 'La lista de la compra está vacía.' : 'No hay notas todavía.',
  );

  readonly form = this.fb.nonNullable.group({
    content: ['', Validators.required],
  });

  ngOnInit(): void {
    this.load();
  }

  setKind(event: MatButtonToggleChange): void {
    this.kind.set(event.value as NoteKind);
    this.load();
  }

  add(): void {
    if (this.form.invalid) {
      return;
    }

    const content = this.form.getRawValue().content.trim();
    this.noteService.create({ content, kind: this.kind(), roomId: null }).subscribe(() => {
      this.form.reset({ content: '' });
      this.load();
    });
  }

  toggle(id: number): void {
    this.noteService.toggle(id).subscribe(() => this.load());
  }

  remove(id: number): void {
    this.noteService.delete(id).subscribe(() => this.load());
  }

  private load(): void {
    this.noteService.loadAll(this.kind()).subscribe();
  }
}
