import { describe, expect, it } from 'vitest';
import { LoadingService } from './loading.service';

describe('LoadingService', () => {
  it('activa el indicador mientras hay peticiones activas', () => {
    const service = new LoadingService();

    expect(service.loading()).toBe(false);

    service.start();
    expect(service.loading()).toBe(true);

    service.start();
    service.stop();
    expect(service.loading()).toBe(true);

    service.stop();
    expect(service.loading()).toBe(false);
  });

  it('no baja de cero si se para más veces de las que se empieza', () => {
    const service = new LoadingService();

    service.stop();
    service.stop();

    expect(service.loading()).toBe(false);
  });
});
