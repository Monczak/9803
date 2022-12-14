using System;
using System.Collections.Generic;
using System.Text;
using NineEightOhThree.VirtualCPU.Utilities;

namespace NineEightOhThree.VirtualCPU.Assembly.Assembler
{
    public class CompiledStatement
    {
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
                        switch (op.IsDefined)
                        {
                            case true when op.Number <= 0xFF:
                                bytes += 1;
                                break;
                            case true:
                                bytes += 2;
                                break;
                            default:
                                if (AddressingMode is VirtualCPU.AddressingMode.ZeroPage
                                    or VirtualCPU.AddressingMode.ZeroPageX or VirtualCPU.AddressingMode.ZeroPageY
                                    or VirtualCPU.AddressingMode.IndexedIndirect or VirtualCPU.AddressingMode.IndirectIndexed 
                                    or VirtualCPU.AddressingMode.Relative)
                                    bytes += 1;
                                else
                                    bytes += 2;
                                break;
                        }
                    }
                }

                return bytes;
            }
        }

        public List<byte> Bytes
        {
            get
            {
                List<byte> bytes = new();
                if (Opcode is not null) bytes.Add(Opcode.Value);
                
                void Add8Bit(Operand op)
                {
                    if (op.Number != null) bytes.Add((byte)op.Number.Value);
                    else throw new InternalErrorException("Operand number was null");
                }

                void Add16Bit(Operand op)
                {
                    if (op.Number != null) bytes.AddRange(BitUtils.ToLittleEndian(op.Number.Value));
                    else throw new InternalErrorException("Operand number was null");
                }

                if (Operands is not null)
                {
                    foreach (Operand op in Operands)
                    {
                        switch (op.IsDefined)
                        {
                            case true when op.Number <= 0xFF:
                                Add8Bit(op);
                                break;
                            case true:
                                Add16Bit(op);
                                break;
                            default:
                                if (AddressingMode is VirtualCPU.AddressingMode.ZeroPage
                                    or VirtualCPU.AddressingMode.ZeroPageX or VirtualCPU.AddressingMode.ZeroPageY
                                    or VirtualCPU.AddressingMode.IndexedIndirect or VirtualCPU.AddressingMode.IndirectIndexed 
                                    or VirtualCPU.AddressingMode.Relative)
                                {
                                    if (AddressingMode is VirtualCPU.AddressingMode.Relative)
                                    {
                                        // TODO: Handle relative addressing separately
                                        Add8Bit(op);
                                    }
                                    else
                                    {
                                        Add16Bit(op);
                                    }
                                }
                                else
                                    Add16Bit(op);
                                break;
                        }
                    }
                }

                return bytes;
            }
        }

        public CompiledStatement(ushort startPC, (AddressingMode addressingMode, CPUInstructionMetadata metadata)? instrData)
        {
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

        public CompiledStatement(ushort startPC, (AddressingMode addressingMode, CPUInstructionMetadata metadata)? metadata, List<Operand> operands) : this(startPC, metadata)
        {
            Operands = operands;
        }

        public CompiledStatement(ushort startPC, (AddressingMode addressingMode, CPUInstructionMetadata metadata)? metadata, params Operand[] operands) : this(startPC, metadata, new List<Operand>(operands))
        {
        }

        public override string ToString()
        {
            return
                $"{(Opcode is null ? "No opcode" : $"Opcode {Opcode:X2}")}, {(Operands is null ? "no operands" : new StringBuilder().AppendJoin(" ", Operands).ToString())}";
        }
    }
}