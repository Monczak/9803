using System;
using System.Collections.Generic;
using System.Text;
using NineEightOhThree.VirtualCPU.Assembly.Assembler.Statements;
using NineEightOhThree.VirtualCPU.Utilities;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class CompiledStatement
    {
        public AbstractStatement Stmt { get; }
        public byte? Opcode { get; }
        public AddressingMode? AddressingMode { get; }
        public List<Operand> Operands { get; }
        
        public ushort StartProgramCounter { get; }

        public ushort ByteCount
        {
            get
            {
                ushort bytes = 0;
                if (Opcode is not null) bytes += 1;

                if (Operands is not null)
                {
                    foreach (Operand op in Operands)
                    {
                        if (op.IsDefined)
                        {
                            if (AddressingMode is VirtualCPU.AddressingMode.ZeroPage
                                or VirtualCPU.AddressingMode.ZeroPageX or VirtualCPU.AddressingMode.ZeroPageY
                                or VirtualCPU.AddressingMode.IndexedIndirect or VirtualCPU.AddressingMode.IndirectIndexed 
                                or VirtualCPU.AddressingMode.Immediate or VirtualCPU.AddressingMode.Relative)
                                bytes += 1;
                            else
                                bytes += 2;
                        }
                        else
                        {
                            bytes += 2;
                        }
                    }
                }

                return bytes;
            }
        }

        public OperationResult<List<byte>> GetBytes(ushort programCounter)
        {
            List<byte> bytes = new();
            if (Opcode is not null) bytes.Add(Opcode.Value);

            OperationResult Add8Bit(Operand op)
            {
                if (op.Number != null)
                {
                    if (op.Number.Value > 0xFF)
                        return OperationResult.Error(SyntaxErrors.OperandNotByte(op.Token));
                    
                    bytes.Add((byte)op.Number.Value);
                }
                else 
                    throw new InternalErrorException("Operand number was null");
                
                return OperationResult.Success();
            }
            
            OperationResult Add8BitRelative(Operand op)
            {
                if (op.Number != null)
                {
                    if (op.LabelRef is null && op.Number > 0xFF)
                        return OperationResult.Error(SyntaxErrors.OperandNotByte(op.Token));
                    
                    bytes.Add((byte)(op.Number.Value - programCounter - ByteCount));
                }
                else
                    throw new InternalErrorException("Operand number was null");
                
                return OperationResult.Success();
            }

            OperationResult Add16Bit(Operand op)
            {
                if (op.Number != null) bytes.AddRange(BitUtils.ToLittleEndian(op.Number.Value));
                else throw new InternalErrorException("Operand number was null");
                
                return OperationResult.Success();
            }

            if (Operands is not null)
            {
                foreach (Operand op in Operands)
                {
                    if (op.IsDefined)
                    {
                        if (AddressingMode is VirtualCPU.AddressingMode.ZeroPage
                            or VirtualCPU.AddressingMode.ZeroPageX or VirtualCPU.AddressingMode.ZeroPageY
                            or VirtualCPU.AddressingMode.IndexedIndirect or VirtualCPU.AddressingMode.IndirectIndexed
                            or VirtualCPU.AddressingMode.Immediate or VirtualCPU.AddressingMode.Relative)
                        {
                            if (AddressingMode is VirtualCPU.AddressingMode.Relative)
                            {
                                // TODO: Warn if distance to label exceeds byte limit
                                var result = Add8BitRelative(op);
                                if (result.Failed) return OperationResult<List<byte>>.Error(result.TheError);
                            }
                            else
                            {
                                var result = Add8Bit(op);
                                if (result.Failed) return OperationResult<List<byte>>.Error(result.TheError);
                            }
                        }
                        else
                        {
                            var result = Add16Bit(op);
                            if (result.Failed) return OperationResult<List<byte>>.Error(result.TheError);
                        }
                    }
                    else
                    {
                        throw new InternalErrorException("Operand still undefined");
                    }
                }
            }

            return OperationResult<List<byte>>.Success(bytes);
        }

        public CompiledStatement(AbstractStatement origStmt, ushort startPC, (AddressingMode addressingMode, CPUInstructionMetadata metadata)? instrData)
        {
            Stmt = origStmt;
            StartProgramCounter = startPC;
            if (instrData.HasValue)
            {
                Opcode = instrData.Value.metadata.Opcode;
                AddressingMode = instrData.Value.addressingMode;
            }
            else
            {
                Opcode = null;
                AddressingMode = null;
            }
            Operands = null;
        }

        public CompiledStatement(AbstractStatement origStmt, ushort startPC, (AddressingMode addressingMode, CPUInstructionMetadata metadata)? metadata, List<Operand> operands) : this(origStmt, startPC, metadata)
        {
            Operands = operands;
        }

        public CompiledStatement(AbstractStatement origStmt, ushort startPC, (AddressingMode addressingMode, CPUInstructionMetadata metadata)? metadata, params Operand[] operands) : this(origStmt, startPC, metadata, new List<Operand>(operands))
        {
        }

        public override string ToString()
        {
            return
                $"{(Opcode is null ? "No opcode" : $"Opcode {Opcode:X2}")}, {(Operands is null ? "no operands" : new StringBuilder().AppendJoin(" ", Operands).ToString())}";
        }
    }
}