// CIL example program.
//
// To assemble:
//                 ilasm add.il

.assembly 'example' { }

.class public 'Test' extends ['mscorlib']'System'.'Object' {
    .method public static void 'whatever'() {
        .entrypoint
        ldc.i4 2
        ldc.i4 3
        add
        call void class ['mscorlib']'System'.'Console'::'WriteLine'(int32)
        ret
    }
}
