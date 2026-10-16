import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { Room, RoomRequest } from '../../../core/models/room.model';

export const ROOM_ICONS = [
  'weekend',
  'chair',
  'kitchen',
  'countertops',
  'bed',
  'king_bed',
  'bathtub',
  'child_care',
  'desk',
  'tv',
  'yard',
  'garage',
  'balcony',
  'laundry',
  'cleaning_services',
  'meeting_room',
];

export interface RoomFormDialogData {
  room?: Room;
}

@Component({
  selector: 'app-room-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  template: `
    <h2 mat-dialog-title>{{ isEdit ? 'Editar habitación' : 'Nueva habitación' }}</h2>
    <form [formGroup]="form" (ngSubmit)="submit()">
      <mat-dialog-content>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Nombre</mat-label>
          <input matInput formControlName="name" placeholder="Cocina" maxlength="40" />
          @if (form.controls.name.hasError('required')) {
            <mat-error>El nombre es obligatorio</mat-error>
          } @else if (form.controls.name.hasError('minlength')) {
            <mat-error>Introduce al menos 2 caracteres</mat-error>
          }
        </mat-form-field>

        <p class="icon-label">Icono</p>
        <div class="icon-grid">
          @for (icon of icons; track icon) {
            <button
              type="button"
              mat-icon-button
              class="icon-option"
              [class.icon-option--selected]="form.controls.icon.value === icon"
              (click)="form.controls.icon.setValue(icon)"
              [attr.aria-label]="icon"
            >
              <mat-icon>{{ icon }}</mat-icon>
            </button>
          }
        </div>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" mat-dialog-close>Cancelar</button>
        <button mat-flat-button type="submit" [disabled]="form.invalid">Guardar</button>
      </mat-dialog-actions>
    </form>
  `,
  styles: `
    .full-width {
      width: 100%;
    }

    .icon-label {
      margin: 0.5rem 0 0.25rem;
      font: var(--mat-sys-label-large);
      color: var(--mat-sys-on-surface-variant);
    }

    .icon-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(44px, 1fr));
      gap: 0.25rem;
    }

    .icon-option {
      border-radius: 12px;
    }

    .icon-option--selected {
      background: var(--mat-sys-secondary-container);
      color: var(--mat-sys-on-secondary-container);
    }
  `,
})
export class RoomFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<RoomFormDialog, RoomRequest>);
  private readonly data = inject<RoomFormDialogData>(MAT_DIALOG_DATA, { optional: true }) ?? {};

  readonly icons = ROOM_ICONS;
  readonly isEdit = !!this.data.room;

  readonly form = this.fb.nonNullable.group({
    name: [this.data.room?.name ?? '', [Validators.required, Validators.minLength(2)]],
    icon: [this.data.room?.icon || 'weekend', Validators.required],
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.dialogRef.close({
      name: value.name.trim(),
      icon: value.icon,
    });
  }
}
