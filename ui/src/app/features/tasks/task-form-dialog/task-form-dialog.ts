import { Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { RecurrenceType, TaskItem, TaskRequest } from '../../../core/models/task.model';

export interface TaskFormDialogData {
  roomId: number;
  task?: TaskItem;
}

@Component({
  selector: 'app-task-form-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
  ],
  template: `
    <h2 mat-dialog-title>{{ isEdit ? 'Editar tarea' : 'Nueva tarea' }}</h2>
    <form [formGroup]="form" (ngSubmit)="submit()">
      <mat-dialog-content>
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Tarea</mat-label>
          <input matInput formControlName="title" placeholder="Fregar los platos" maxlength="80" />
          @if (form.controls.title.hasError('required')) {
            <mat-error>El título es obligatorio</mat-error>
          } @else if (form.controls.title.hasError('minlength')) {
            <mat-error>Introduce al menos 2 caracteres</mat-error>
          }
        </mat-form-field>

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Fecha (opcional)</mat-label>
          <input matInput type="date" formControlName="date" />
        </mat-form-field>

        @if (form.controls.date.value) {
          <div class="time-row">
            <mat-form-field appearance="outline">
              <mat-label>Desde</mat-label>
              <input matInput type="time" formControlName="startTime" />
            </mat-form-field>
            <mat-form-field appearance="outline">
              <mat-label>Hasta</mat-label>
              <input matInput type="time" formControlName="endTime" />
            </mat-form-field>
          </div>

          @if (timeInvalid()) {
            <p class="error">La hora de fin debe ser posterior a la de inicio.</p>
          }

          <p class="hint">Sin horas ocupa el día completo; con horas aparece en esa franja del calendario.</p>
        }

        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Repetir</mat-label>
          <mat-select formControlName="recurrence">
            @for (option of recurrenceOptions; track option.value) {
              <mat-option [value]="option.value">{{ option.label }}</mat-option>
            }
          </mat-select>
        </mat-form-field>
      </mat-dialog-content>

      <mat-dialog-actions align="end">
        <button mat-button type="button" mat-dialog-close>Cancelar</button>
        <button mat-flat-button type="submit" [disabled]="form.invalid || timeInvalid()">Guardar</button>
      </mat-dialog-actions>
    </form>
  `,
  styles: `
    .full-width {
      width: 100%;
    }

    .time-row {
      display: flex;
      gap: 0.75rem;

      mat-form-field {
        flex: 1;
      }
    }

    .error {
      margin: -0.75rem 0 0.75rem;
      color: var(--mat-sys-error);
      font: var(--mat-sys-body-small);
    }

    .hint {
      margin: -0.25rem 0 1rem;
      color: var(--mat-sys-on-surface-variant);
      font: var(--mat-sys-body-small);
    }
  `,
})
export class TaskFormDialog {
  private readonly fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<TaskFormDialog, TaskRequest>);
  readonly data = inject<TaskFormDialogData>(MAT_DIALOG_DATA);

  readonly isEdit = !!this.data.task;

  readonly recurrenceOptions: { value: RecurrenceType; label: string }[] = [
    { value: 'none', label: 'No repetir' },
    { value: 'daily', label: 'Cada día' },
    { value: 'weekly', label: 'Cada semana' },
    { value: 'monthly', label: 'Cada mes' },
  ];

  readonly form = this.fb.nonNullable.group({
    title: [this.data.task?.title ?? '', [Validators.required, Validators.minLength(2)]],
    date: [this.toDateInput(this.data.task?.dueDate ?? null)],
    startTime: [this.toTimeInput(this.data.task?.dueDate ?? null)],
    endTime: [this.toTimeInput(this.data.task?.endDate ?? null)],
    recurrence: [this.data.task?.recurrence ?? 'none'],
  });

  private readonly formValue = toSignal(this.form.valueChanges, {
    initialValue: this.form.getRawValue(),
  });

  readonly timeInvalid = computed(() => {
    const value = this.formValue();
    return !!value.startTime && !!value.endTime && value.endTime <= value.startTime;
  });

  submit(): void {
    if (this.form.invalid || this.timeInvalid()) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const { dueDate, endDate } = this.buildDates(value.date, value.startTime, value.endTime);

    this.dialogRef.close({
      roomId: this.data.roomId,
      title: value.title.trim(),
      isCompleted: this.data.task?.isCompleted ?? false,
      dueDate,
      endDate,
      recurrence: value.recurrence as RecurrenceType,
    });
  }

  private buildDates(
    date: string,
    startTime: string,
    endTime: string,
  ): { dueDate: string | null; endDate: string | null } {
    if (!date) {
      return { dueDate: null, endDate: null };
    }

    if (!startTime) {
      return { dueDate: `${date}T00:00:00`, endDate: null };
    }

    return {
      dueDate: `${date}T${startTime}:00`,
      endDate: `${date}T${endTime || this.addHour(startTime)}:00`,
    };
  }

  private addHour(time: string): string {
    const [hours, minutes] = time.split(':').map(Number);
    const total = Math.min(hours * 60 + minutes + 60, 24 * 60 - 1);
    return `${String(Math.floor(total / 60)).padStart(2, '0')}:${String(total % 60).padStart(2, '0')}`;
  }

  private toDateInput(value: string | null): string {
    return value ? value.substring(0, 10) : '';
  }

  private toTimeInput(value: string | null): string {
    if (!value) {
      return '';
    }
    const time = value.substring(11, 16);
    return time === '00:00' ? '' : time;
  }
}
