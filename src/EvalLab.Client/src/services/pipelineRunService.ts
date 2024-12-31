import { InjectionKey } from 'vue';
import { createGenericService, Entity, GenericService } from './shared';

export type NewPipelineRun = {
  pipelineId: string;
  input: string;
};

export type PipelineRun = NewPipelineRun &
  Entity & {
    output: string;
  };

type PipelineRunsService = GenericService<NewPipelineRun, PipelineRun>;

type PipelineRunsServiceKeyType = InjectionKey<PipelineRunsService>;

export const PipelineRunsServiceKey: PipelineRunsServiceKeyType = Symbol('PipelineRunsService');

const BASE_URL = '/api/pipelines/runs';

export const pipelineRunsService: PipelineRunsService = Object.freeze(
  createGenericService(BASE_URL, createPipelineRun),
);

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export function createPipelineRun(data: any): PipelineRun {
  return {
    pipelineId: data.pipelineId,
    id: data.id,
    name: data.name,
    input: data.input,
    output: data.output,
    createdDate: new Date(data.createdDate),
    updatedDate: new Date(data.updatedDate),
  };
}
