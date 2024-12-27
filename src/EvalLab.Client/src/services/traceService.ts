import { InjectionKey } from 'vue';
import { createGenericService, Entity, GenericService } from './shared';

export type Span = {
  parentId?: string;
  id: string;
  name: string;
  duration: number;
  start: number;
  end: number;
  attributes: Record<string, string>;
};

export type Trace = Entity & {
  runId: string;
  duration: number;
  start: number;
  end: number;
  spans: Span[];
};

type TracesService = Pick<GenericService<Trace, Trace, Trace>, 'get'>;

type TracesServiceKeyType = InjectionKey<TracesService>;

export const TracesServiceKey: TracesServiceKeyType = Symbol('TracesService');

const BASE_URL = '/api/traces';

const { get } = createGenericService(BASE_URL, createTrace, createTrace);

export const tracesService: TracesService = Object.freeze({ get });

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createTrace(data: any): Trace {
  return {
    id: data.id,
    runId: data.runId,
    name: data.name,
    duration: data.duration,
    start: data.start,
    end: data.end,
    spans: data.spans.map(createSpan),
    createdDate: new Date(data.createdDate),
    updatedDate: new Date(data.updatedDate),
  };
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
function createSpan(data: any): Span {
  return {
    parentId: data.parentId,
    id: data.id,
    name: data.name,
    duration: data.duration,
    start: data.start,
    end: data.end,
    attributes: data.attributes,
  };
}
